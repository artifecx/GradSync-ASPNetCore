using Services.ServiceModels;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IApplicationService
    {
        Task AddApplicationAsync(ApplicationViewModel model);
        Task UpdateApplicationAsync(ApplicationViewModel model);
        Task DeleteApplicationAsync(string id);
        Task<PaginatedList<ApplicationViewModel>> GetApplicationsAsync(
            int pageIndex, int pageSize, string search, string statusFilter);
    }
}
