using Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Data.Interfaces
{
    public interface IApplicationRepository
    {
        Task AddApplicationAsync(Application application);
        Task UpdateApplicationAsync(Application application);
        Task DeleteApplicationAsync(Application application);
        Task<List<Application>> GetAllApplications(bool includes);
    }
}
