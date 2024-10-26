using Services.ServiceModels;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IApplicationService
    {
        Task SendApplicationAsync(string userId, string jobId);
        Task UpdateApplicationAsync(string userId, string applicationId, string applicationStatusTypeId);
        Task ArchiveApplicationAsync(string userId, string applicationId);
        Task<PaginatedList<ApplicationViewModel>> GetAllApplicationsAsync(ApplicationFilter filters);
        Task<ApplicationViewModel> GetApplicationByIdAsync(string id);
    }
}
