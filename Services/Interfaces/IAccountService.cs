using Data.Models;
using Microsoft.AspNetCore.Http;
using Services.ServiceModels;
using System.Threading.Tasks;
using static Resources.Constants.Enums;

namespace Services.Interfaces
{
    public interface IAccountService
    {
        LoginResult AuthenticateUser(string email, string password, ref User user);
        void RegisterUser(AccountServiceModel model);
        bool UserExists(string Email);
        bool UserIdExists(string id);
        string GetCurrentUserRole();
        void AddAdmin(User user);
        void AddApplicant(User user);
        void AddRecruiter(User user);
        Task ResetUserPasswordAsync(string id, string type = null);
        Task<string> VerifyUserEmail(string token);
        Task<string> CompleteUserPasswordRequestAsync(string token);
    }
}
