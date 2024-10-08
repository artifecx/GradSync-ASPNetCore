using Data.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data.Interfaces
{
    public interface IJobRepository
    {
        Task<List<Application>> GetAllApplicationsNoIncludesAsync();
        Task<List<Job>> GetAllJobsDepartmentsIncludeAsync();
        Task<List<Job>> GetAllJobsAsync();
        Task<List<Job>> GetRecruiterJobsAsync(string userId);
        Task AddJobAsync(Job job);
        Task UpdateJobAsync(Job job);
        Task DeleteJobAsync(Job job);
        Task<Job> GetJobByIdAsync(string id);
        Task<List<Company>> GetCompaniesWithListingsAsync();
        Task<List<EmploymentType>> GetEmploymentTypesAsync();
        Task<List<StatusType>> GetStatusTypesAsync();
        Task<List<SetupType>> GetWorkSetupsAsync();
        Task<List<Department>> GetDepartmentsAsync();
        Task<List<YearLevel>> GetYearLevelsAsync();
        Task<List<Skill>> GetSkillsAsync();
    }
}
