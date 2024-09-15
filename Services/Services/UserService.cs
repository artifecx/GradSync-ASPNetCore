using Data.Interfaces;
using Data.Models;
using Services.Interfaces;
using Services.Manager;
using Services.ServiceModels;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Data.Repositories;
using System.Threading.Tasks;

namespace Services.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAdminRepository _adminRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserService"/> class.
        /// </summary>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="adminRepository">The admin repository.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="httpContextAccessor">The HTTP context accessor.</param>
        public UserService(IUserRepository userRepository, IAdminRepository adminRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _adminRepository = adminRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<UserViewModel>> GetAllAsync()
        {
            var users = await _userRepository.GetAllUsersAsync(); 

            var data = users.Select(s => new UserViewModel
            {
                UserId = s.UserId,
                Email = s.Email,
                Name = s.Name,
                Password = new string('*', s.Password.Length),
                RoleId = s.RoleId,
            }).ToList();

            return data;
        }

        public async Task<UserViewModel> GetUserAsync(string userId)
        {
            var model = await _userRepository.FindByIdAsync(userId);
            return new UserViewModel
            {
                UserId = model.UserId,
                Email = model.Email,
                Name = model.Name,
                Password = new string('*', model.Password.Length),
                RoleId = model.RoleId,
            };
        }

        public async Task UpdateAsync(UserViewModel model)
        {
            var updatedUser = await _userRepository.FindByIdAsync(model.UserId);
            _mapper.Map(model, updatedUser);

            updatedUser.Password = PasswordManager.EncryptPassword(updatedUser.Password);
            await _userRepository.UpdateAsync(updatedUser);
        }

        public async Task DeleteAsync(string userId) => 
            await _userRepository.DeleteAsync(userId);

        #region Get Methods
        public async Task<List<Role>> GetRoles() =>
            await _userRepository.GetAllRolesAsync();
        #endregion
    }
}
