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
        Task<ApplicantDetailsDto> GetApplicantDetailsByIdAsync(string id);
        Task<List<ApplicantDetailsDto>> GetAllApplicantDetailsAsync(HashSet<string> departmentIds);
        Task<JobDetailsDto> GetJobDetailsByIdAsync(string id);
        Task<List<JobDetailsDto>> GetAllJobDetailsAsync(string departmentId);
    }
}
