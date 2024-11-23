using Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Data.Interfaces
{
    public interface IJobMatchRepository
    {
        Task AddJobApplicantMatchesAsync(List<JobApplicantMatch> matches);
        Task<object> GetApplicantDetailsByIdAsync(string id);
        Task<List<object>> GetAllApplicantDetailsAsync();
        Task<object> GetJobDetailsByIdAsync(string id);
        Task<List<object>> GetAllJobDetailsAsync();
    }
}
