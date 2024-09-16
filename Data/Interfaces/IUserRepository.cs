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
        Task UpdateAsync(User user);
        Task DeleteAsync(string UserId);
        Task<bool> UserExistsAsync(string UserId);
        Task<User> FindByIdAsync(string id);
        Task<Role> FindRoleByIdAsync(string id);
        Task<List<Role>> GetAllRolesAsync();
    }
}
