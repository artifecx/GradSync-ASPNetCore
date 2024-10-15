using Data.Models;
using Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IUserPreferencesService
    {
        Task<UserPreferencesViewModel> GetUserPreferencesAsync(string userId);
        Task UpdateUserPreferencesAsync(UserPreferencesViewModel model);
        Task UpdateUserPassword(UserPreferencesViewModel model);
        Task<KeyValuePair<string, string>> GetUserPreferenceByKeyAsync(string userId, string key);
    }
}
