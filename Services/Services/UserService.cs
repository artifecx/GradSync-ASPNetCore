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
        private readonly IUserRepository _repository;
        private readonly IAccountService _accountService;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserService"/> class.
        /// </summary>
        /// <param name="repository">The user repository.</param>
        /// <param name="accountService">The account service.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="httpContextAccessor">The HTTP context accessor.</param>
        public UserService(IUserRepository repository, IAccountService accountService, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _repository = repository;
            _accountService = accountService;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<PaginatedList<UserViewModel>> GetAllAsync(string sortBy, string filterBy, string role, bool? verified, int pageIndex, int pageSize)
        {
            var claimsPrincipal = _httpContextAccessor.HttpContext.User;
            var currentUserIsSuper = claimsPrincipal.FindFirst("IsSuperAdmin")?.Value;
            var currentUserId = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var users = _mapper.Map<List<UserViewModel>>(await _repository.GetAllUsersAsync());

            if(currentUserIsSuper != "true")
                users = users.Where(u => u.RoleId != "Admin").ToList();
            else
                users = users.Where(u => u.UserId != currentUserId).ToList();

            if (!string.IsNullOrEmpty(filterBy))
            {
                users = users.Where(d => d.FirstName.Contains(filterBy, StringComparison.OrdinalIgnoreCase) ||
                                   (d.LastName.Contains(filterBy, StringComparison.OrdinalIgnoreCase)) ||
                                   (d.Email.Contains(filterBy, StringComparison.OrdinalIgnoreCase)))
                             .ToList();
            }

            if (!string.IsNullOrEmpty(role))
            {
                users = users.Where(d => d.RoleId.Contains(role, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (verified.HasValue)
            {
                users = users.Where(d => d.IsVerified == verified).ToList();
            }

            users = sortBy switch
            {
                "name_desc" => users.OrderByDescending(t => t.LastName).ToList(),
                "email_desc" => users.OrderByDescending(t => t.Email).ToList(),
                "email" => users.OrderBy(t => t.Email).ToList(),
                _ => users.OrderBy(t => t.LastName).ToList(),
            };

            var count = users.Count;
            var items = users.Skip((pageIndex - 1) * pageSize).Take(pageSize);

            return new PaginatedList<UserViewModel>(items, count, pageIndex, pageSize);
        }

        public async Task<UserViewModel> GetUserAsync(string userId) =>
            _mapper.Map<UserViewModel>(await _repository.FindUserByIdAsync(userId));

        public async Task AddAsync(UserViewModel model)
        {
            var userModel = _mapper.Map<AccountServiceModel>(model);
            userModel.Password = "defpass"; //Default password
            userModel.AsRecruiter = model.RoleId == "Recruiter";

            if (model.RoleId == "NLO" || model.RoleId == "Admin")
            {
                if (_accountService.UserExists(userModel.Email))
                    throw new UserException("User already exists!");

                var user = _mapper.Map<User>(userModel);
                user.UserId = Guid.NewGuid().ToString();
                user.RoleId = model.RoleId;
                user.JoinDate = DateTime.Now;
                user.Password = PasswordManager.EncryptPassword(userModel.Password);
                user.IsVerified = true;

                _repository.AddUser(user);
                _accountService.AddAdmin(user);
            }
            else
            {
                _accountService.RegisterUser(userModel);
            }
        }

        public async Task UpdateAsync(UserViewModel model)
        {
            var user = await _repository.FindUserByIdAsync(model.UserId);
            bool firstNameChanged = !string.Equals(model.FirstName, user.FirstName, StringComparison.Ordinal);
            bool middleNameChanged = !string.Equals(model.MiddleName, user.MiddleName, StringComparison.Ordinal);
            bool lastNameChanged = !string.Equals(model.LastName, user.LastName, StringComparison.Ordinal);
            bool suffixChanged = !string.Equals(model.Suffix, user.Suffix, StringComparison.Ordinal);
            bool emailChanged = !string.Equals(model.Email, user.Email, StringComparison.Ordinal);
            bool roleChanged = !string.Equals(model.RoleId, user.RoleId, StringComparison.Ordinal);
            bool hasChanges = firstNameChanged || middleNameChanged || lastNameChanged || suffixChanged || emailChanged || roleChanged;

            if(!hasChanges)
                throw new UserException("No changes detected.");

            model.IsVerified = user.IsVerified && !emailChanged; //TODO: send verification email when changed

            if (roleChanged)
            {
                model.RoleId = await SetRole(user.UserId, user.RoleId, model.RoleId);
                _mapper.Map(model, user);
                await _repository.UpdateUserAsync(user);

                switch(user.RoleId)
                {
                    case "Admin":
                    case "NLO":
                        _accountService.AddAdmin(user);
                        break;
                    case "Applicant":
                        _accountService.AddApplicant(user);
                        break;
                    case "Recruiter":
                        _accountService.AddRecruiter(user);
                        break;
                }
            }
            _mapper.Map(model, user);
            await _repository.UpdateUserAsync(user);
        }

        private async Task<string> SetRole(string userId, string currentRole, string newRole)
        {
            string[] administrativeRoles = { "Admin", "NLO" };
            if ((administrativeRoles.Contains(currentRole) && !administrativeRoles.Contains(newRole)) ||
                (!administrativeRoles.Contains(currentRole) && administrativeRoles.Contains(newRole)))
                throw new UserException($"Illegal switch from {currentRole} to {newRole}.");

            if (administrativeRoles.Contains(currentRole))
            {
                await _repository.DeleteAdminAsync(userId);
            }
            else
            {
                if(currentRole == "Applicant")
                    await _repository.DeleteApplicantAsync(userId);
                else if (currentRole == "Recruiter")
                    await _repository.DeleteRecruiterAsync(userId);
            }

            return newRole;
        }

        public async Task ResetPasswordAsync(string id)
        {
            var user = await _repository.FindUserByIdAsync(id);
            user.Password = PasswordManager.EncryptPassword("defpass"); //TODO: randomize and send via email

            await _repository.UpdateUserAsync(user);
        }

        public async Task DeleteAsync(string userId) => 
            await _repository.DeleteUserAsync(userId);

        public async Task<List<Role>> GetRolesAsync() =>
            await _repository.GetAllRolesAsync();
    }
}
