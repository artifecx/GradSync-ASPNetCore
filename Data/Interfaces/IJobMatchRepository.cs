using Data.Dtos;
using Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Data.Interfaces
{
    public interface IJobMatchRepository
    {
        Task AddJobApplicantMatchesAsync(List<JobApplicantMatch> matches);
        Task DeleteJobApplicantMatchesByApplicantIdAsync(string applicantId);
        Task DeleteJobApplicantMatchesByJobIdAsync(string jobId);
        Task<JobMatchingApplicantDetailsDto> GetApplicantDetailsByIdAsync(string id);
        Task<List<JobMatchingApplicantDetailsDto>> GetAllApplicantDetailsAsync(HashSet<string> departmentIds);
        Task<JobMatchingJobDetailsDto> GetJobDetailsByIdAsync(string id);
        Task<List<JobMatchingJobDetailsDto>> GetAllJobDetailsAsync(string departmentId);
    }
}
