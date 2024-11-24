using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Data.Interfaces;
using Data.Models;
using Services.Interfaces;
using Services.Manager;
using Services.ServiceModels;
using static Resources.Constants.UserRoles;
using static Resources.Messages.ErrorMessages;
using static Services.Exceptions.UserExceptions;


namespace Services.Services
{
    /// <summary>
    /// Service class for handling operations related to <see cref="User"/>.
    /// </summary>
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
        public UserService
            (
                IUserRepository repository, 
                IAccountService accountService,
                IMapper mapper,
                IHttpContextAccessor httpContextAccessor
            )
        {
            _repository = repository;
            _accountService = accountService;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        #region Get Methods
        /// <summary>
        /// Retrieves filtered and sorted users asynchronously.
        /// </summary>
        /// <param name="sortBy">User defined sort order.</param>
        /// <param name="search">User defined searchfilter.</param>
        /// <param name="role">User defined role filter.</param>
        /// <param name="verified">User defined filter of verified status.</param>
        /// <param name="pageIndex">Page index.</param>
        /// <param name="pageSize">Page size.</param>
        /// <returns>A <see cref="Task{T}"/> representing the asynchronous operation. 
        /// The task result contains a paginated list of <see cref="UserViewModel"/>.</returns>
        public async Task<PaginatedList<UserViewModel>> GetAllUsersAsync(FilterServiceModel filters)
        {
            var sortBy = filters.SortBy;
            var search = filters.Search;
            var role = filters.Role;
            var verified = filters.Verified;
            var pageIndex = filters.PageIndex;
            var pageSize = filters.PageSize;

            var claimsPrincipal = _httpContextAccessor.HttpContext.User;
            var currentUserIsSuper = claimsPrincipal.FindFirst("IsSuperAdmin")?.Value;
            var currentUserId = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var users = _mapper.Map<List<UserViewModel>>(await _repository.GetAllUsersAsync());

            if(currentUserIsSuper != "true")
            {
                users = users.Where(u => u.RoleId != Role_Admin).ToList();
            }
            else
            {
                users = users.Where(u => u.UserId != currentUserId).ToList();
            } 

            if (!string.IsNullOrEmpty(search))
            {
                users = users.Where
                    (
                        d => d.FirstName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                        d.LastName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                        d.Email.Contains(search, StringComparison.OrdinalIgnoreCase)
                    ).ToList();
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
                "name_asc" => users.OrderBy(t => t.LastName).ToList(),
                "email_desc" => users.OrderByDescending(t => t.Email).ToList(),
                "email_asc" => users.OrderBy(t => t.Email).ToList(),
                _ => users.OrderBy(t => t.LastName).ToList(),
            };

            var count = users.Count;
            var items = users.Skip((pageIndex - 1) * pageSize).Take(pageSize);

            return new PaginatedList<UserViewModel>(items, count, pageIndex, pageSize);
        }

        /// <summary>
        /// Retrieves a <see cref="User"/> by its identifier asynchronously.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>A <see cref="Task{T}"/> representing the asynchronous operation. 
        /// The task result contains the <see cref="UserViewModel"/>.</returns>
        public async Task<UserViewModel> GetUserAsync(string userId) =>
            _mapper.Map<UserViewModel>(await _repository.GetUserByIdAsync(userId));
        #endregion

        #region CRUD Methods
        /// <summary>
        /// Adds a new user asynchronously.
        /// </summary>
        /// <param name="model">The user view model.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <exception cref="UserException">Thrown when a similar user already exists.</exception>
        public async Task AddUserAsync(UserViewModel model)
        {
            var userModel = _mapper.Map<AccountServiceModel>(model);
            userModel.Password = "defpass"; //Default password
            userModel.AsRecruiter = model.RoleId == Role_Recruiter;

            if (model.RoleId == Role_NLO || model.RoleId == Role_Admin)
            {
                if (_accountService.UserExists(userModel.Email))
                    throw new UserException(Error_UserExists);

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

            await Task.CompletedTask;
        }

        /// <summary>
        /// Updates an existing user asynchronously.
        /// </summary>
        /// <param name="model">The user view model.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <exception cref="UserException">Thrown when no changes were detected.</exception>
        public async Task UpdateUserAsync(UserViewModel model)
        {
            var user = await _repository.GetUserByIdAsync(model.UserId);
            bool firstNameChanged = !string.Equals(model.FirstName, user.FirstName, StringComparison.Ordinal);
            bool middleNameChanged = !string.Equals(model.MiddleName, user.MiddleName, StringComparison.Ordinal);
            bool lastNameChanged = !string.Equals(model.LastName, user.LastName, StringComparison.Ordinal);
            bool suffixChanged = !string.Equals(model.Suffix, user.Suffix, StringComparison.Ordinal);
            bool emailChanged = !string.Equals(model.Email, user.Email, StringComparison.Ordinal);
            bool roleChanged = !string.Equals(model.RoleId, user.RoleId, StringComparison.Ordinal);
            bool hasChanges = firstNameChanged || middleNameChanged || lastNameChanged || suffixChanged || emailChanged || roleChanged;

            if(!hasChanges)
                throw new UserException(Error_NoChanges);

            model.IsVerified = user.IsVerified && !emailChanged; //TODO: send verification email when changed

            if (roleChanged)
            {
                model.RoleId = await SetUserRole(user.UserId, user.RoleId, model.RoleId);
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

        /// <summary>
        /// Deletes an existing user asynchronously.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task DeleteUserAsync(string userId) =>
            await _repository.DeleteUserAsync(userId);
        #endregion

        #region Helper Methods
        /// <summary>
        /// Resets a user's password asynchronously and sends an email to the user.
        /// </summary>
        /// <param name="id">The user identifier.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task ResetUserPasswordAsync(string id) =>
            await _accountService.ResetUserPasswordAsync(id, "id");

        /// <summary>
        /// Sets a user <see cref="Role"/> asynchronously. 
        /// Hard deletes the previous role and adds the new role.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="currentRole">The user's current role.</param>
        /// <param name="newRole">The user's new role.</param>
        /// <returns>A <see cref="string"/> containing the new user role.</returns>
        /// <exception cref="UserException">Thrown when going to an administrative role from a non-administrative role and vice versa</exception>
        private async Task<string> SetUserRole(string userId, string currentRole, string newRole)
        {
            var administrativeRoles = new HashSet<string> { Role_Admin, Role_NLO };
            if ((administrativeRoles.Contains(currentRole) && !administrativeRoles.Contains(newRole)) ||
                (!administrativeRoles.Contains(currentRole) && administrativeRoles.Contains(newRole)))
                throw new UserException(string.Format(Error_UserIllegalRoleSwitch, currentRole, newRole));
            
            if (administrativeRoles.Contains(currentRole))
            {
                await _repository.DeleteAdminAsync(userId);
            }
            else
            {
                if(currentRole == Role_Applicant)
                    await _repository.DeleteApplicantAsync(userId);
                else if (currentRole == Role_Recruiter)
                    await _repository.DeleteRecruiterAsync(userId);
            }

            return newRole;
        }
        #endregion
    }
}
