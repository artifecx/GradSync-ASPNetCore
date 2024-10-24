using Data.Models;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

public interface IAvatarService
{
    Task AddAvatarAsync(Avatar avatar);
    Task<Avatar> GetAvatarByIdAsync(string avatarId);
    Task DeleteAvatarAsync(string avatarId);
    Task UploadAvatarAsync(IFormFile avatarFile, string userId);


}
