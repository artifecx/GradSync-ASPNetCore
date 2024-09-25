using Data.Interfaces;
using Data.Models;
using Services.Interfaces;
using Services.ServiceModels;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Resources.Messages;
using System.Globalization;

namespace Services.Services
{
    /// <summary>
    /// Service class for handling operations related to teams.
    /// </summary>
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _repository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IJobRepository _jobRepository;

        public CompanyService(
            ICompanyRepository repository,
            IMapper mapper,
            ILogger<CompanyService> logger,
            IHttpContextAccessor httpContextAccessor,
            IJobRepository jobRepository)
        {
            _repository = repository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _jobRepository = jobRepository;
        }

        public async Task AddCompanysync(CompanyViewModel model)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateCompanyAsync(CompanyViewModel model)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteCompanyAsync(string id)
        {
            throw new NotImplementedException();
        }

        #region Get Methods        
        public async Task<PaginatedList<CompanyViewModel>> GetAllCompaniesAsync(
            string sortBy, string search, bool? verified, bool? hasValidMOA, int pageIndex, int pageSize)
        {
            var companies = _mapper.Map<List<CompanyViewModel>>(await _repository.GetAllCompaniesNoFilesAsync());
            companies = await FilterAndSortCompanies(companies, sortBy, search, verified, hasValidMOA);

            var count = companies.Count;
            var items = companies.Skip((pageIndex - 1) * pageSize).Take(pageSize);

            return new PaginatedList<CompanyViewModel>(items, count, pageIndex, pageSize);
        }

        public async Task<PaginatedList<CompanyViewModel>> GetRecruiterCompanyAsync(string userId)
        {
            throw new NotImplementedException();
        }

        private async Task<List<CompanyViewModel>> FilterAndSortCompanies(List<CompanyViewModel> companies, string sortBy, string search, bool? verified, bool? hasValidMOA)
        {
            if (!string.IsNullOrEmpty(search))
            {
                companies = companies.Where(c => c.Name.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (verified.HasValue)
            {
                if (verified.Value)
                {
                    companies = companies.Where(c => c.IsVerified).ToList();
                }
                else
                {
                    companies = companies.Where(c => !c.IsVerified).ToList();
                }
            }

            if (hasValidMOA.HasValue) {
                DateTime currentDate = DateTime.Today;
                if (hasValidMOA.Value)
                {
                    companies = companies.Where(c => c.MemorandumOfAgreement != null
                        && c.MemorandumOfAgreement.ValidityStart <= currentDate
                        && (c.MemorandumOfAgreement.ValidityEnd == null || c.MemorandumOfAgreement.ValidityEnd >= currentDate))
                        .ToList();
                }
                else
                {
                    companies = companies.Where(c => c.MemorandumOfAgreement == null
                        || c.MemorandumOfAgreement.ValidityStart > currentDate
                        || (c.MemorandumOfAgreement.ValidityEnd != null && c.MemorandumOfAgreement.ValidityEnd < currentDate))
                        .ToList();
                }
            }

            companies = sortBy switch
            {
                "name_desc" => companies.OrderByDescending(c => c.Name).ToList(),
                "jobs" => companies.OrderBy(c => c.Recruiters.Count(r => r.Jobs.Any(j => !j.IsArchived))).ToList(),
                "jobs_desc" => companies.OrderByDescending(c => c.Recruiters.Count(r => r.Jobs.Any(j => !j.IsArchived))).ToList(),
                _ => companies.OrderBy(c => c.Name).ToList(),
            };

            return companies;
        }

        public async Task<CompanyViewModel> GetCompanyByIdAsync(string id) =>
            _mapper.Map<CompanyViewModel>(await _repository.GetCompanyByIdAsync(id));
        #endregion Get Methods
    }
}
