using Data.Models;
using Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IUserService
    {
        Task<PaginatedList<UserViewModel>> GetAllUsersAsync(FilterServiceModel filters);
        Task<UserViewModel> GetUserAsync(string userId);
        Task AddUserAsync(UserViewModel model);
        Task UpdateUserAsync(UserViewModel model);
        Task ResetUserPasswordAsync(string id);
        Task DeleteUserAsync(string userId);
    }
}
