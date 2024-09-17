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
using Resources.Messages;
using static Services.Exceptions.TeamExceptions;
using static Services.Exceptions.UserExceptions;

namespace Services.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAccountService _accountService;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserService"/> class.
        /// </summary>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="accountService">The account service.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="httpContextAccessor">The HTTP context accessor.</param>
        public UserService(IUserRepository userRepository, IAccountService accountService, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _accountService = accountService;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<PaginatedList<UserViewModel>> GetAllAsync(string sortBy, string filterBy, string role, int pageIndex, int pageSize)
        {
            var claimsPrincipal = _httpContextAccessor.HttpContext.User;
            var currentUserIsSuper = claimsPrincipal.FindFirst("IsSuperAdmin")?.Value;
            var currentUserId = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var users = await _userRepository.GetAllUsersAsync();
            var data = users.Select(s => new UserViewModel
            {
                UserId = s.UserId,
                Email = s.Email,
                Name = s.Name,
                Password = new string('*', s.Password.Length),
                RoleId = s.RoleId,
            }).ToList();

            if(currentUserIsSuper != "true")
                data = data.Where(u => u.RoleId != "Admin").ToList();
            else
                data = data.Where(u => u.UserId != currentUserId).ToList();

            if (!string.IsNullOrEmpty(filterBy))
            {
                data = data.Where(d => d.Name.Contains(filterBy, StringComparison.OrdinalIgnoreCase) ||
                                   (d.RoleId.Contains(filterBy, StringComparison.OrdinalIgnoreCase)))
                             .ToList();
            }

            if (!string.IsNullOrEmpty(role))
            {
                data = data.Where(d => d.RoleId.Contains(role, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            data = sortBy switch
            {
                "name_desc" => data.OrderByDescending(t => t.Name).ToList(),
                "email_desc" => data.OrderByDescending(t => t.Email).ToList(),
                "email" => data.OrderBy(t => t.Email).ToList(),
                _ => data.OrderBy(t => t.Name).ToList(),
            };

            var count = data.Count;
            var items = data.Skip((pageIndex - 1) * pageSize).Take(pageSize);

            return new PaginatedList<UserViewModel>(items, count, pageIndex, pageSize);
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

        public async Task AddAsync(UserViewModel model)
        {
            var userModel = new AccountServiceModel
            {
                Name = model.Name,
                Email = model.Email,
                Password = "defpass", //Default password
                AsRecruiter = model.RoleId == "Recruiter",
            };

            if(model.RoleId == "NLO" || model.RoleId == "Admin")
            {
                if (_accountService.UserExists(userModel.Email))
                    throw new UserException("User already exists!");

                var user = _mapper.Map<User>(userModel);
                user.UserId = Guid.NewGuid().ToString();
                user.RoleId = model.RoleId;
                user.JoinDate = DateTime.Now;
                user.Password = PasswordManager.EncryptPassword(userModel.Password);

                _userRepository.AddUser(user);
                AddAdmin(user);
            }
            else
            {
                _accountService.RegisterUser(userModel);
            }
        }

        private void AddAdmin(User user)
        {
            _userRepository.AddAdmin(new Admin
            {
                UserId = user.UserId,
                IsSuper = user.RoleId == "Admin",
            });
        }

        public async Task UpdateAsync(UserViewModel model)
        {
            var updatedUser = await _userRepository.FindByIdAsync(model.UserId);
            _mapper.Map(model, updatedUser);

            updatedUser.Password = PasswordManager.EncryptPassword(updatedUser.Password);
            await _userRepository.UpdateAsync(updatedUser);
        }

        public async Task ResetPasswordAsync(string id)
        {
            var user = await _userRepository.FindByIdAsync(id);
            user.Password = PasswordManager.EncryptPassword("defpass"); //TODO: randomize and send via email

            await _userRepository.UpdateAsync(user);
        }

        public async Task DeleteAsync(string userId) => 
            await _userRepository.DeleteAsync(userId);

        public async Task<List<Role>> GetRolesAsync() =>
            await _userRepository.GetAllRolesAsync();
    }
}
