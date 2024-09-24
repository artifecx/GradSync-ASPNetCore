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
        Task DeleteJobAsync(string id);
        Task<PaginatedList<JobViewModel>> GetAllJobsAsync(
            string sortBy, string search, string filterByCompany, 
            string filterByEmploymentType, string filterByStatusType, 
            string filterByWorkSetup, int pageIndex, int pageSize);
        Task<PaginatedList<JobViewModel>> GetRecruiterJobsAsync(
            string sortBy, string search, string filterByCompany,
            string filterByEmploymentType, string filterByStatusType,
            string filterByWorkSetup, int pageIndex, int pageSize);
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
