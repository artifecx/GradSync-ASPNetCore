using AutoMapper;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Data.Interfaces;
using Data.Models;
using Services.Interfaces;
using Services.Manager;
using Services.ServiceModels;
using static Resources.Constants.Enums;
using static Resources.Constants.UserRoles;
using static Resources.Messages.EmailMessages;
using static Resources.Messages.ErrorMessages;
using static Resources.Messages.SuccessMessages;
using static Services.Exceptions.UserExceptions;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace Services.Services
{
    /// <summary>
    /// Service class for handling operations related to <see cref="User"/> accounts such as authentication and verification.
    /// </summary>
    public class AccountService : IAccountService
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEmailService _emailService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountService"/> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="httpContextAccessor">The HTTP context accessor.</param> 
        /// <param name="emailService">The email service.</param>
        public AccountService
            (IUserRepository repository, 
            IMapper mapper, 
            IHttpContextAccessor httpContextAccessor, 
            IEmailService emailService,
            ILogger<AccountService> logger)
        {
            _mapper = mapper;
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
            _emailService = emailService;
        }

        /// <summary>
        /// Authenticates a user for logging in. 
        /// If user is unverified and their token is expired or non-existent, 
        /// it will generate a new token and sends it to their registered email.
        /// </summary>
        /// <param name="email">The user's inputted email.</param>
        /// <param name="password">The user's inputted password.</param>
        /// <param name="user">The reference user model, value is passed back to the controller through ref.</param>
        /// <returns><see cref="LoginResult"/> Success if successfully logged in; otherwise Failed</returns>
        /// <exception cref="UserNotVerifiedException">Thrown when a user trying to login is not verified.</exception>
        public LoginResult AuthenticateUser(string email, string password, ref User user)
        {
            user = new User();
            var passwordKey = PasswordManager.EncryptPassword(password);
            user = _repository.GetUserByEmailAndPasswordAsync(email, passwordKey).Result;

            if (user != null)
            {
                if (!user.IsVerified)
                {
                    if (user.TokenExpiry <= DateTime.Now || string.IsNullOrEmpty(user.Token))
                    {
                        string newToken = GenerateToken(user);
                        _repository.UpdateUserAsync(user).Wait();
                        SendVerificationEmail(newToken, user.FirstName, user.Email);
                    }
                    throw new UserNotVerifiedException(Error_UserNotVerified, user.Email);
                }

                if(user.RoleId != Role_Applicant)
                    user.FromSignUp = user.LastLoginDate == null;
                user.LastLoginDate = DateTime.Now;

                _repository.UpdateUserAsync(user).Wait();
                return LoginResult.Success;
            }
            return LoginResult.Failed;
        }

        /// <summary>
        /// Registers a new user asynchronously and calls <see cref="SendVerificationEmail"/> to send a verification email.
        /// </summary>
        /// <param name="model">The account view model.</param>
        /// <returns><see cref="void"/></returns>
        /// <exception cref="UserException">Thrown when a similar user already exists.</exception>
        public void RegisterUser(AccountServiceModel model)
        {
            if (UserExists(model.Email))
                throw new UserException(Error_UserExists);

            var user = _mapper.Map<User>(model);
            user.FromSignUp = true;
            user.UserId = Guid.NewGuid().ToString();
            user.Password = PasswordManager.EncryptPassword(model.Password);
            user.RoleId = Role_Applicant;
            user.JoinDate = DateTime.Now;

            string token = GenerateToken(user);

            _repository.AddUser(user);

            AddApplicant(user);

            SendVerificationEmail(token, user.FirstName, user.Email);
        }

        /// <summary>
        /// Adds a new <see cref="Admin"/>.
        /// </summary>
        /// <param name="user">The user.</param>
        public void AddAdmin(User user)
        {
            _repository.AddAdmin(new Admin
            {
                UserId = user.UserId,
                IsSuper = user.RoleId == Role_Admin,
            });
        }

        /// <summary>
        /// Adds a new <see cref="Applicant"/>.
        /// </summary>
        /// <param name="user">The user.</param>
        public void AddApplicant(User user)
        {
            _repository.AddApplicant(new Applicant
            {
                UserId = user.UserId,
            });
        }

        /// <summary>
        /// Adds a new <see cref="Recruiter"/>.
        /// </summary>
        /// <param name="user">The user.</param>
        public void AddRecruiter(User user)
        {
            _repository.AddRecruiter(new Recruiter
            {
                UserId = user.UserId,
            });
        }

        /// <summary>
        /// Verifies a user using the provided token asynchronously.
        /// Resends a verification email if the token has expired.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>A <see cref="Task{T}"/> representing the asynchronous operation. 
        /// The task result contains the resulting <see cref="string"/> verification message.</returns>
        public async Task<string> VerifyUserEmail(string token)
        {
            var user = await _repository.GetUserByTokenAsync(token);
            if (user == null || user.IsVerified)
                return Error_UserTokenInvalid;

            if (user.TokenExpiry >= DateTime.Now)
            {
                user.IsVerified = true;
                user.Token = null;
                user.TokenExpiry = null;
                await _repository.UpdateUserAsync(user);
                return Success_UserVerificationSuccess;
            }
            string newToken = GenerateToken(user);
            await _repository.UpdateUserAsync(user);
            SendVerificationEmail(newToken, user.FirstName, user.Email);
            return Error_UserTokenExpired;
        }

        /// <summary>
        /// Resets a user's password asynchronously and sends an email to the user.
        /// </summary>
        /// <param name="id">The user identifier.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <exception cref="UserException">Thrown when a user requests a password reset on an unverified user.</exception>
        public async Task ResetUserPasswordAsync(string id, string type = null)
        {
            var user = string.Equals(type, "id") ? await _repository.GetUserByIdAsync(id) : await _repository.GetUserByEmailAsync(id);
            if (user == null) return;

            if(string.Equals(type, "id"))
            {
                string newPassword = GenerateNewPassword(user);
                await _repository.UpdateUserAsync(user);

                SendNewPasswordToEmail(newPassword, user.FirstName, user.Email);
            }
            else
            {
                bool isAdministrative = string.Equals(user.RoleId, Role_Admin) || string.Equals(user.RoleId, Role_NLO);
                if (isAdministrative) return;

                if (!user.IsVerified) throw new UserException(Error_UserUnverifiedPasswordReset);

                string token = GenerateToken(user);
                await _repository.UpdateUserAsync(user);

                string url = $"https://gradsync.org/resetpassword?token={token}";

                string subject = Email_SubjectUserRequestPasswordReset;
                string body = string.Format(Email_BodyUserRequestPasswordReset, user.FirstName, url);

                _emailService.SendEmail(user.Email, subject, body);
            }
        }

        /// <summary>
        /// Completes a user's password request asynchronously.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>A <see cref="Task{T}"/> representing the asynchronous operation. 
        /// The task result contains the resulting <see cref="string"/> verification message.</returns>
        public async Task<string> CompleteUserPasswordRequestAsync(string token)
        {
            var user = await _repository.GetUserByTokenAsync(token);
            if (user == null || !user.IsVerified)
                return Error_UserTokenInvalid;

            if (user.TokenExpiry >= DateTime.Now)
            {
                string newPassword = GenerateNewPassword(user);
                await _repository.UpdateUserAsync(user);
                SendNewPasswordToEmail(newPassword, user.FirstName, user.Email);
                return Success_UserPasswordResetSuccess;
            }
            user.Token = null;
            user.TokenExpiry = null;
            return Error_UserPasswordTokenExpired;
        }

        #region Helper Methods
        /// <summary>
        /// Checks if a user with the provided email already exists.
        /// </summary>
        /// <returns><see cref="bool"/> true if user exists; otherwise false</returns>
        public bool UserExists(string Email) =>
            _repository.GetAllUsersAsync().Result.Exists(x => x.Email == Email);

        /// <summary>
        /// Checks if a user id exists.
        /// </summary>
        /// <returns><see cref="bool"/> true if user id exists; otherwise false</returns>
        public bool UserIdExists(string id) =>
            _repository.GetAllUsersAsync().Result.Exists(x => x.UserId == id);

        /// <summary>
        /// Retrieves the currently logged in user's role.
        /// </summary>
        /// <returns>The <see cref="string"/> role name</returns>
        public string GetCurrentUserRole() =>
            _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;

        /// <summary>
        /// A helper method to generate a token for a user with a 1-hour expiry.
        /// </summary>
        /// <param name="user">The user model.</param>
        /// <returns>A randomly generated 64-character <see cref="string"/> token.</returns>
        private static string GenerateToken(User user)
        {
            string token = PasswordGenerator.GeneratePassword(64);
            user.Token = token;
            user.TokenExpiry = DateTime.Now.AddHours(1);
            return token;
        }

        /// <summary>
        /// A helper method to generate a new password for a user.
        /// </summary>
        /// <param name="user">The user model.</param>
        /// <returns>A randomly generated 12-character <see cref="string"/> password.</returns>
        private static string GenerateNewPassword(User user)
        {
            string newPassword = PasswordGenerator.GeneratePassword();
            user.Password = PasswordManager.EncryptPassword(newPassword);
            return newPassword;
        }

        /// <summary>
        /// A helper method that sends a formatted verification email to the user.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="firstname">The user's first name.</param>
        /// <param name="email">The user's email.</param>
        /// <returns><see cref="void"/></returns>
        private void SendVerificationEmail(string token, string firstname, string email)
        {
            string url = $"https://gradsync.org/verify?token={token}";
            string subject = Email_SubjectUserRegistration;
            string body = string.Format(Email_BodyUserRegistration, firstname, url);
            _emailService.SendEmail(email, subject, body);
        }

        /// <summary>
        /// A helper method that sends a formatted email containing their new password to the user.
        /// </summary>
        /// <param name="newPassword">The new randomly generated password.</param>
        /// <param name="firstname">The user's first name.</param>
        /// <param name="email">The user's email.</param>
        /// <returns><see cref="void"/></returns>
        private void SendNewPasswordToEmail(string newPassword, string firstname, string email)
        {
            string subject = Email_SubjectUserPasswordReset;
            string body = string.Format(Email_BodyUserPasswordReset, firstname, newPassword);
            _emailService.SendEmail(email, subject, body);
        }
        #endregion
    }
}
