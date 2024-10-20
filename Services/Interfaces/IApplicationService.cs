using Data.Models;
using Services.ServiceModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IApplicationService
    {
        Task AddApplicationAsync(ApplicationViewModel model);
        Task UpdateApplicationAsync(ApplicationViewModel model);
        Task DeleteApplicationAsync(string id);
        Task<PaginatedList<ApplicationViewModel>> GetAllApplicationsAsync(ApplicationFilter filters);
    }
}
