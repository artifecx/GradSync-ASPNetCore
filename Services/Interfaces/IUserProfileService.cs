using Data.Models;
using Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IUserProfileService
    {
        Task<UserProfileViewModel> GetUserProfileAsync(string userId, string roleId);
        Task UpdateUserProfileAsync(UserProfileViewModel model);
        Task UpdateUserPassword(UserProfileViewModel model);
        Task<KeyValuePair<string, string>> GetUserPreferenceByKeyAsync(string userId, string key);
    }
}
