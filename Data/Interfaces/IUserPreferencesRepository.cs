using System.Collections.Generic;
using System.Threading.Tasks;

namespace Data.Interfaces
{
    public interface IUserPreferencesRepository
    {
        Dictionary<string, string> GetUserPreferences(string userId);
        Task UpdateUserPreferencesAsync(string userId, Dictionary<string, string> updatedPreferences);
        Task<KeyValuePair<string, string>> GetUserPreferenceByKeyAsync(string userId, string key);
    }   
}
