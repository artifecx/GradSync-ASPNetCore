﻿using Data.Interfaces;
using Data.Models;
using Services.Interfaces;
using Services.Manager;
using Services.ServiceModels;
using AutoMapper;
using System;
using static Resources.Constants.Enums;
using static Services.Exceptions.UserExceptions;
using Microsoft.AspNetCore.Http;
using static Resources.Messages.ErrorMessages;
using static Resources.Constants.UserRoles;

namespace Services.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountService"/> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="mapper">The mapper.</param>
        public AccountService(IUserRepository repository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
        }

        public void RegisterUser(AccountServiceModel model)
        {
            if (UserExists(model.Email))
                throw new UserException(Error_UserExists);

            var user = _mapper.Map<User>(model);
            user.UserId = Guid.NewGuid().ToString();
            user.Password = PasswordManager.EncryptPassword(model.Password);
            user.RoleId = model.AsRecruiter ? Role_Recruiter : Role_Applicant;
            user.JoinDate = DateTime.Now;

            _repository.AddUser(user);

            if (model.AsRecruiter)
                AddRecruiter(user);
            else
                AddApplicant(user);
        }

        public void AddAdmin(User user)
        {
            _repository.AddAdmin(new Admin
            {
                UserId = user.UserId,
                IsSuper = user.RoleId == Role_Admin,
            });
        }

        public void AddApplicant(User user)
        {
            _repository.AddApplicant(new Applicant
            {
                UserId = user.UserId,
            });
        }

        public void AddRecruiter(User user)
        {
            _repository.AddRecruiter(new Recruiter
            {
                UserId = user.UserId,
            });
        }

        public LoginResult AuthenticateUser(string email, string password, ref User user)
        {
            user = new User();
            var passwordKey = PasswordManager.EncryptPassword(password);
            user = _repository.GetAllUsersAsync().Result.Find(x => x.Email == email && x.Password == passwordKey);
            
            if(user != null)
            {
                if (!user.IsVerified)
                    throw new UserNotVerifiedException(Error_UserNotVerified);

                user.LastLoginDate = DateTime.Now;
                _repository.UpdateUserAsync(user).Wait();
                return LoginResult.Success;
            }
            return LoginResult.Failed;
        }

        public bool UserExists(string Email) =>
            _repository.GetAllUsersAsync().Result.Exists(x => x.Email == Email);

        public string GetCurrentUserRole() =>
            _httpContextAccessor.HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
    }
}
