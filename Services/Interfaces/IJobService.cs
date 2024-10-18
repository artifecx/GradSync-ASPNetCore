using Data.Models;
using Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IJobService
    {
        Task AddJobAsync(JobViewModel model);
        Task UpdateJobAsync(JobViewModel model);
        Task ArchiveJobAsync(string id);
        Task UnarchiveJobAsync(JobServiceModel model);
        Task<List<JobViewModel>> GetAllJobsAsync();
        Task<PaginatedList<JobViewModel>> GetAllJobsAsync(
            string sortBy, string search, string filterByCompany,
            List<string> filterByEmploymentType, string filterByStatusType,
            List<string> filterByWorkSetup, int pageIndex, int pageSize,
            string filterByDatePosted = null, string filterBySalary = null, string archived = null);
        Task<PaginatedList<JobViewModel>> GetRecruiterJobsAsync(
            string sortBy, string search, string filterByCompany,
            List<string> filterByEmploymentType, string filterByStatusType,
            List<string> filterByWorkSetup, int pageIndex, int pageSize, string archived = null);
        Task<JobViewModel> GetJobByIdAsync(string id);
        Task<List<Company>> GetCompaniesWithListingsAsync();
        Task<List<EmploymentType>> GetEmploymentTypesAsync();
        Task<List<StatusType>> GetStatusTypesAsync();
        Task<List<SetupType>> GetWorkSetupsAsync();
        Task<List<Department>> GetDepartmentsAsync();
        Task<List<YearLevel>> GetYearLevelsAsync();
        Task<List<Skill>> GetSkillsAsync();
    }
}
