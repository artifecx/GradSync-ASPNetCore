using Data.Interfaces;
using Data.Models;
using Services.Interfaces;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System;
using Data.Repositories;

namespace Services.Services
{
    public class AvatarService : IAvatarService
    {
        private readonly IAvatarRepository _avatarRepository;
        private readonly IUserRepository _userRepository;

        public AvatarService(IAvatarRepository avatarRepository, IUserRepository userRepository)
        {
            _avatarRepository = avatarRepository;
            _userRepository = userRepository;
        }

        public async Task AddAvatarAsync(Avatar avatar)
        {
            await _avatarRepository.AddAvatarAsync(avatar);
        }

        public async Task<Avatar> GetAvatarByIdAsync(string avatarId)
        {
            return await _avatarRepository.GetAvatarByIdAsync(avatarId);
        }

        public async Task DeleteAvatarAsync(string avatarId)
        {
            await _avatarRepository.DeleteAvatarAsync(avatarId);
        }

        public async Task UploadAvatarAsync(IFormFile avatarFile, string userId)
        {
            if (avatarFile == null || avatarFile.Length == 0)
            {
                throw new ArgumentException("Invalid avatar file.");
            }

            // Process the file (e.g., save it to a database or file system)
            using (var memoryStream = new MemoryStream())
            {
                await avatarFile.CopyToAsync(memoryStream);
                var fileContent = memoryStream.ToArray();

                // Save the file content to the database or file system
                // Example: await _repository.SaveAvatarAsync(userId, fileContent);
            }
        } 
        private async Task<byte[]> ConvertToByteArrayAsync(IFormFile file)
        {
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }


    }
}
