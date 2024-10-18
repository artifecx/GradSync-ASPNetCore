﻿using AutoMapper;
using Data.Interfaces;
using Data.Models;
using Services.Interfaces;
using Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using static Services.Exceptions.CompanyExceptions;

namespace Services.Services
{
    /// <summary>
    /// Service class for handling operations related to teams.
    /// </summary>
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _repository;
        private readonly IMapper _mapper;
        private readonly IAccountService _accountService;

        public CompanyService(
            ICompanyRepository repository,
            IMapper mapper,
            ILogger<CompanyService> logger,
            IAccountService accountService)
        {
            _repository = repository;
            _mapper = mapper;
            _accountService = accountService;
        }

        public async Task AddCompanyAsync(CompanyViewModel model)
        {
            var company = _mapper.Map<Company>(model);
            if (_repository.CompanyExists(company))
                throw new CompanyException("Company with the same name already exists.");

            company.CompanyId = Guid.NewGuid().ToString();

            if (string.Equals(_accountService.GetCurrentUserRole(), "NLO"))
                company.IsVerified = true;
            else
                throw new CompanyException("Could not verify user to enable adding companies.");

            await _repository.AddCompanyAsync(company);
        }

        public async Task UpdateCompanyAsync(CompanyViewModel model)
        {
            var company = await _repository.GetCompanyByIdAsync(model.CompanyId);
            if (company == null)
                throw new CompanyException("Company not found.");

            _mapper.Map(model, company);

            if (_repository.CompanyExists(company, company.CompanyId))
                throw new CompanyException("Company with the same name already exists.");

            if (!_repository.HasChanges(company))
                throw new CompanyException("No changes detected.");

            await _repository.UpdateCompanyAsync(company);
        }
        public async Task<CompanyViewModel> GetRecruiterCompanyAsync(string userId)
        {
            var recruiter = await _repository.GetRecruiterByIdAsync(userId);
            var company = await _repository.GetCompanyByIdAsync(recruiter.CompanyId);
            return _mapper.Map<CompanyViewModel>(company);
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

        private static async Task<List<CompanyViewModel>> FilterAndSortCompanies(List<CompanyViewModel> companies, string sortBy, string search, bool? verified, bool? hasValidMOA)
        {
            if (!string.IsNullOrEmpty(search))
            {
                companies = companies
                            .Where(c => 
                                c.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                c.Address.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                c.ContactNumber.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                c.ContactEmail.Contains(search, StringComparison.OrdinalIgnoreCase))
                            .ToList();
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

        public async Task ArchiveCompanyAsync(string companyId) =>
            await _repository.ArchiveCompanyAsync(companyId);

        public async Task VerifyCompanyAsync(string companyId)
        {
            var company = await _repository.GetCompanyByIdAsync(companyId);
            if (company.IsVerified)
                throw new CompanyException("Company is already verified.");

            company.IsVerified = true;
            await _repository.UpdateCompanyAsync(company);
        }

        public async Task InvalidateCompanyAsync(string companyId)
        {
            var company = await _repository.GetCompanyByIdAsync(companyId);
            if (!company.IsVerified)
                throw new CompanyException("Company is currently unverified.");

            company.IsVerified = false;
            await _repository.UpdateCompanyAsync(company);
        }

        public async Task UpdateMOAStatusAsync(IFormFile file, string type)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateCompanyLogoAsync(IFormFile file, string type)
        {
            throw new NotImplementedException();
        }

        public async Task ReassignRecruiterAsync(string companyId, string recruiterId) =>
            throw new NotImplementedException();

        public async Task AssignRecruiterAsync(string companyId, string recruiterId) =>
            throw new NotImplementedException();
    }
}
