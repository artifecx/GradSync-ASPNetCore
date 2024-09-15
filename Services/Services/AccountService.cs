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
            if (UserExists(model.UserId))
                throw new UserException("User already exists!"); // TODO: Change to custom exception

            User user = new User{
                UserId = Guid.NewGuid().ToString(),
                Email = model.UserId,
                Password = PasswordManager.EncryptPassword(model.Password),
                RoleId = "Applicant",
                Name = model.Name,
                JoinDate = DateTime.Now
            };
            _repository.AddUser(user);
        }

        public LoginResult AuthenticateUser(string email, string password, ref User user)
        {
            user = new User();
            var passwordKey = PasswordManager.EncryptPassword(password);
            var allUsers = _repository.GetAllUsersAsync().Result;
            user = _repository.GetAllUsersAsync().Result.Find(x => x.Email == email && x.Password == passwordKey);

            return user != null ? LoginResult.Success : LoginResult.Failed;
        }

        public bool UserExists(string Email) =>
            _repository.GetAllUsersAsync().Result.Exists(x => x.Email == Email);
    }
}
