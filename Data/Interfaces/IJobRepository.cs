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
        Task<List<Job>> GetAllJobsProgramsIncludeAsync();
        Task<List<JobApplicantMatch>> GetApplicantFeaturedJobsAsync(string userId);
        Task<List<Job>> GetAllJobsAsync();
        Task<List<Job>> GetArchivedJobsAsync();
        Task<List<Job>> GetRecruiterJobsAsync(string userId);
        Task<List<Job>> GetRecruiterArchivedJobsAsync(string userId);
        Task AddJobAsync(Job job);
        Task UpdateJobAsync(Job job);
        Task<Job> GetJobByIdAsync(string id, bool? track);
        Task<Job> GetJobByIdAsync(string id, string isArchived);
        Task<List<EmploymentType>> GetEmploymentTypesAsync();
        Task<List<StatusType>> GetStatusTypesAsync();
        Task<List<SetupType>> GetWorkSetupsAsync();
        Task<List<Program>> GetProgramsAsync();
        Task<List<YearLevel>> GetYearLevelsAsync();
        Task<List<Skill>> GetSkillsAsync();
        bool HasChanges(Job job);
    }
}
