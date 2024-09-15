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
        Task<List<UserViewModel>> GetAllAsync();
        Task<UserViewModel> GetUserAsync(string userId);
        Task UpdateAsync(UserViewModel model);
        Task DeleteAsync(string userId);
        Task<List<Role>> GetRoles();
    }
}
