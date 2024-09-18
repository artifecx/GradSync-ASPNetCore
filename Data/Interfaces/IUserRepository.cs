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
        void AddUser(User user);
        void AddApplicant(Applicant applicant);
        void AddRecruiter(Recruiter recruiter);
        void AddAdmin(Admin nlo);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(string userId);
        Task DeleteAdminAsync(string adminId);
        Task DeleteApplicantAsync(string applicantId);
        Task DeleteRecruiterAsync(string recruiterId);
        Task<User> FindUserByIdAsync(string id);
        Task<Role> FindRoleByIdAsync(string id);
        Task<List<Role>> GetAllRolesAsync();
    }
}
