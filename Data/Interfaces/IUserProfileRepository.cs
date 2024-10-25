using Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Data.Interfaces
{
    public interface IUserProfileRepository
    {
        Dictionary<string, string> GetUserProfile(string userId);
        Task UpdateUserProfileAsync(string userId, Dictionary<string, string> updatedPreferences);
        Task<KeyValuePair<string, string>> GetUserPreferenceByKeyAsync(string userId, string key);
        Task<User> GetApplicantProfileByUserId(string userId);
        Task<User> GetRecruiterProfileByUserId(string userId);
    }   
}
