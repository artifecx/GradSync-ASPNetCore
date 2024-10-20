using Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Interfaces
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllUsersAsync();
        Task<List<User>> GetAllUsersNoIncludesAsync();
        Task<User> GetUserByIdAsync(string id);
        Task<User> GetUserByEmailAsync(string email);
        Task<User> GetUserByEmailAndPasswordAsync(string email, string passwordKey);
        Task<User> GetUserByTokenAsync(string token);
        void AddUser(User user);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(string userId);
        void AddAdmin(Admin admin);
        Task DeleteAdminAsync(string adminId);
        void AddApplicant(Applicant applicant);
        Task DeleteApplicantAsync(string applicantId);
        void AddRecruiter(Recruiter recruiter);
        Task DeleteRecruiterAsync(string recruiterId);

        bool IsSuperAdmin(string adminId);
    }
}
