using Data.Models;
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
        void AddAdmin(User user);
        void AddApplicant(User user);
        void AddRecruiter(User user);
    }
}
