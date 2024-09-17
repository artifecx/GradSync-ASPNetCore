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
        Task<PaginatedList<UserViewModel>> GetAllAsync(string sortBy, string filterBy, string role, int pageIndex, int pageSize);
        Task<UserViewModel> GetUserAsync(string userId);
        Task AddAsync(UserViewModel model);
        Task UpdateAsync(UserViewModel model);
        Task ResetPasswordAsync(string id);
        Task DeleteAsync(string userId);
        Task<List<Role>> GetRolesAsync();
    }
}
