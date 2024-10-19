using Data.Interfaces;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using FuzzySharp;
using System;
using System.ComponentModel.Design;

namespace Data.Repositories
{
    public class CompanyRepository : BaseRepository, ICompanyRepository
    {
        public CompanyRepository(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        private IQueryable<Company> GetCompaniesWithIncludes()
        {
            return this.GetDbSet<Company>()
                        .Where(c => !c.IsArchived)
                        .Include(c => c.MemorandumOfAgreement)
                        .Include(c => c.CompanyLogo)
                        .Include(c => c.Recruiters)
                            .ThenInclude(r => r.Jobs)
                        .Include(c => c.Recruiters)
                            .ThenInclude(r => r.User);
        }

        public async Task<List<Company>> GetAllCompaniesNoFilesAsync()
        {
            return await this.GetDbSet<Company>()
                .Where(c => !c.IsArchived)
                .Include(c => c.MemorandumOfAgreement)
                .Include(c => c.Recruiters)
                    .ThenInclude(r => r.Jobs)
                .Include(c => c.Recruiters)
                    .ThenInclude(r => r.User)
                .Select(c => new Company
                {
                    CompanyId = c.CompanyId,
                    ContactEmail = c.ContactEmail,
                    ContactNumber = c.ContactNumber,
                    Name = c.Name,
                    Description = c.Description,
                    Address = c.Address,
                    CompanyLogoId = c.CompanyLogoId,
                    MemorandumOfAgreementId = c.MemorandumOfAgreementId,
                    IsArchived = c.IsArchived,
                    IsVerified = c.IsVerified,
                    MemorandumOfAgreement = c.MemorandumOfAgreement != null ? new MemorandumOfAgreement
                    {
                        MemorandumOfAgreementId = c.MemorandumOfAgreement.MemorandumOfAgreementId,
                        FileContent = null,
                        FileName = null,
                        FileType = null,
                        UploadedDate = c.MemorandumOfAgreement.UploadedDate,
                        ValidityStart = c.MemorandumOfAgreement.ValidityStart,
                        ValidityEnd = c.MemorandumOfAgreement.ValidityEnd
                    } : null,
                    Recruiters = c.Recruiters.Select(r => new Recruiter
                    {
                        UserId = r.UserId,
                        User = new User
                        {
                            FirstName = r.User.FirstName,
                            LastName = r.User.LastName,
                            Email = r.User.Email,
                        },
                        Jobs = r.Jobs.Select(j => new Job
                        {
                            JobId = j.JobId,
                            Title = j.Title,
                            IsArchived = j.IsArchived,
                        }).ToList()
                    }).ToList()
                }).AsNoTracking().ToListAsync();
        }

        public async Task<List<Company>> GetAllCompaniesAsync() =>
            await GetCompaniesWithIncludes().AsNoTracking().ToListAsync();

        public async Task AddCompanyAsync(Company company)
        {
            await this.GetDbSet<Company>().AddAsync(company);
            await UnitOfWork.SaveChangesAsync();
        }

        public async Task UpdateCompanyAsync(Company company)
        {
            this.GetDbSet<Company>().Update(company);
            await UnitOfWork.SaveChangesAsync();
        }

        public async Task SetRecruiterCompany(Recruiter recruiter) =>
            this.GetDbSet<Recruiter>().Update(recruiter);

        public async Task ArchiveCompanyAsync(string companyId)
        {
            var recruiterIds = await GetDbSet<Recruiter>()
                .Where(r => r.CompanyId == companyId)
                .Select(r => r.UserId)
                .AsNoTracking().ToListAsync();

            await GetDbSet<Job>()
                .Where(j => recruiterIds.Contains(j.PostedById))
                .ExecuteUpdateAsync(s => s.SetProperty(j => j.IsArchived, true));

            await GetDbSet<Company>()
                .Where(c => c.CompanyId == companyId)
                .ExecuteUpdateAsync(s => s.SetProperty(c => c.IsArchived, true));
        }

        public async Task<Company> GetCompanyByIdAsync(string id) =>
            await GetCompaniesWithIncludes().FirstOrDefaultAsync(c => c.CompanyId == id);
    
        public bool HasChanges(Company company)
        {
            var entry = this.GetDbSet<Company>().Entry(company);
            return entry.Properties.Any(p => p.IsModified);
        }

        public bool CompanyExists(Company company, string excludeCompanyId = null)
        {
            var dbSet = this.GetDbSet<Company>();
            string normalizedInputName = NormalizeCompanyName(company.Name);
            string prefix = company.Name.Substring(0, Math.Min(3, company.Name.Length)).ToLowerInvariant();
            var companies = dbSet
                .Where(c => !c.IsArchived && c.CompanyId != excludeCompanyId && c.Name.ToLower().StartsWith(prefix))
                .Select(c => new { c.CompanyId, c.Name })
                .AsNoTracking()
                .ToList();

            foreach (var c in companies)
            {
                string normalizedDbName = NormalizeCompanyName(c.Name);
                if (AreNamesSimilar(normalizedInputName, normalizedDbName))
                {
                    return true;
                }
            }
            return false;
        }

        public async Task<Recruiter> GetRecruiterByIdAsync(string userId, bool track) 
        {
            var query = track ? this.GetDbSet<Recruiter>() : this.GetDbSet<Recruiter>().AsNoTracking();
            return await query.FirstOrDefaultAsync(c => c.UserId == userId);
        }

        private static string NormalizeCompanyName(string name)
        {
            return name
                .Replace(" ", "")
                .Replace("-", "")
                .Replace(".", "")
                .Replace(",", "")
                .ToLowerInvariant();
        }

        private static bool AreNamesSimilar(string name1, string name2)
        {
            int similarityRatio = Fuzz.TokenSetRatio(name1, name2);
            return similarityRatio >= 80;
        }
    }
}
