using Data.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data.Interfaces
{
    public interface ICompanyRepository
    {
        Task<List<Company>> GetAllCompaniesNoFilesAsync();
        Task<List<Company>> GetAllCompaniesAsync();
        Task AddCompanyAsync(Company company);
        Task UpdateCompanyAsync(Company company);
        Task ArchiveCompanyAsync(string companyId);
        Task<Company> GetCompanyByIdAsync(string id);
        bool HasChanges(Company company);
        bool CompanyExists(Company company, string excludeCompanyId = null);
        Task<Recruiter> GetRecruiterByIdAsync(string userId);
    }
}
