using Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Data.Interfaces
{
    public interface IApplicationRepository
    {
        Task AddApplicationAsync(Application application);
        Task UpdateApplicationAsync(Application application);
        Task<List<Application>> GetAllApplicationsAsync(bool includes);
        Task<Application> GetApplicationByIdAsync(string id);
        Task<List<Application>> GetAllApplicationsByUserAsync(string userId);
    }
}
