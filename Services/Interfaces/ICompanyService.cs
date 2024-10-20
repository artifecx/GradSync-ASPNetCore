using Data.Models;
using Microsoft.AspNetCore.Http;
using Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface ICompanyService
    {
        Task<PaginatedList<CompanyViewModel>> GetAllCompaniesAsync(
            string sortBy, string search, bool? verified, bool? hasValidMOA, int pageIndex, int pageSize);
        Task<CompanyViewModel> GetCompanyByIdAsync(string id);
        Task AddCompanyAsync(CompanyViewModel model);
        Task UpdateCompanyAsync(CompanyViewModel model);
        Task ArchiveCompanyAsync(string companyId);
        Task VerifyCompanyAsync(string companyId);
        Task InvalidateCompanyAsync(string companyId);
        Task UpdateMOAStatusAsync(IFormFile file, string type);
        Task UpdateCompanyLogoAsync(IFormFile file, string type);
        Task ReassignRecruiterAsync(string companyId, string recruiterId);
        Task AssignRecruiterAsync(string companyId, string recruiterId);
        Task<CompanyViewModel> GetCompanyByRecruiterId(string userId);
        Task<List<Company>> GetCompaniesWithListingsAsync();
    }
}
