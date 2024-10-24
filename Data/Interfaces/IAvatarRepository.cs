using Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Data.Interfaces
{
    public interface IAvatarRepository
    {
        Task<List<Avatar>> GetAllAvatarsAsync(); // Retrieve all avatars
        Task<Avatar> GetAvatarByIdAsync(string id); // Get a specific avatar by ID
        Task AddAvatarAsync(Avatar avatar); // Add a new avatar
        Task UpdateAvatarAsync(Avatar avatar); // Update an existing avatar
        Task DeleteAvatarAsync(string id); // Delete an avatar by ID
        bool HasChanges(Avatar avatar); // Check if there are unsaved changes
    }
}
