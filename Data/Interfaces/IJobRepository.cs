using Data.Dtos;
using Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Data.Interfaces
{
    public interface IJobRepository
    {
        Task<List<Job>> GetJobsAsync(ISpecification<Job> specification, bool track);
        Task<Job> GetJobAsync(ISpecification<Job> specification, bool track);
        Task<List<Application>> GetAllApplicationsNoIncludesAsync();
        Task<List<Job>> GetAllJobsProgramsIncludeAsync();
        Task<List<JobApplicantMatch>> GetApplicantFeaturedJobsAsync(string userId);
        Task<ApplicantViewDto> GetApplicantDetailsAsync(string applicantId);
        Task AddJobAsync(Job job);
        Task UpdateJobAsync(Job job);
        bool HasChanges(Job job);
    }
}
