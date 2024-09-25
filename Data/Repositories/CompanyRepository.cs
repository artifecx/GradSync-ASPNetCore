using Data.Interfaces;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;

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
                        .Include(c => c.CompanyLogo);
        }

        public async Task<List<Company>> GetAllCompaniesNoFilesAsync()
        {
            return await this.GetDbSet<Company>()
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

        public async Task DeleteCompanyAsync(Company company)
        {
            company.IsArchived = true;
            this.GetDbSet<Company>().Update(company);
            await UnitOfWork.SaveChangesAsync();
        }

        public async Task<Company> GetCompanyByIdAsync(string id) =>
            await GetCompaniesWithIncludes().FirstOrDefaultAsync(c => c.CompanyId == id);
    }
}
