using Data.Interfaces;
using Data.Models;
using Services.Interfaces;
using Services.Manager;
using Services.ServiceModels;
using AutoMapper;
using System;
using System.IO;
using System.Linq;
using static Resources.Constants.Enums;
using static Services.Exceptions.UserExceptions;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace Services.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountService"/> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="mapper">The mapper.</param>
        public AccountService(IUserRepository repository, IMapper mapper)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public void RegisterUser(AccountServiceModel model)
        {
            if (UserExists(model.Email))
                throw new UserException("User already exists!");

            User user = new User{
                UserId = Guid.NewGuid().ToString(),
                Email = model.Email,
                Password = PasswordManager.EncryptPassword(model.Password),
                RoleId = model.AsRecruiter ? "Recruiter" : "Applicant",
                Name = model.Name,
                JoinDate = DateTime.Now
            };
            _repository.AddUser(user);
        }

        public LoginResult AuthenticateUser(string email, string password, ref User user)
        {
            user = new User();
            var passwordKey = PasswordManager.EncryptPassword(password);
            user = _repository.GetAllUsersAsync().Result.Find(x => x.Email == email && x.Password == passwordKey);
            
            if(user != null)
            {
                user.LastLoginDate = DateTime.Now;
                _repository.UpdateAsync(user).Wait();
                return LoginResult.Success;
            }
            return LoginResult.Failed;
        }

        public bool UserExists(string Email) =>
            _repository.GetAllUsersAsync().Result.Exists(x => x.Email == Email);
    }
}
