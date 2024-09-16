using Data.Interfaces;
using Data.Models;
using Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserRepository"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work.</param>
        public UserRepository(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        public async Task<List<User>> GetAllUsersAsync() =>
            await this.GetDbSet<User>().Include(u => u.Role).ToListAsync();

        public void AddUser(User user)
        {
            this.GetDbSet<User>().Add(user);
            UnitOfWork.SaveChanges();
        }

        public void AddApplicant(Applicant applicant)
        {
            this.GetDbSet<Applicant>().Add(applicant);
            UnitOfWork.SaveChanges();
        }

        public void AddRecruiter(Recruiter recruiter)
        {
            this.GetDbSet<Recruiter>().Add(recruiter);
            UnitOfWork.SaveChanges();
        }

        public async Task UpdateAsync(User user)
        {
            this.GetDbSet<User>().Update(user);
            await UnitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(string UserId)
        {
            var userToDelete = await this.GetDbSet<User>().FirstOrDefaultAsync(u => u.UserId == UserId);
            if (userToDelete != null)
            {
                // TODO: Soft delete, and if last admin, return error
                this.GetDbSet<User>().Remove(userToDelete);
                await UnitOfWork.SaveChangesAsync();
            }
        }

        #region Helper Methods
        public async Task<bool> UserExistsAsync(string UserId) =>
            await this.GetDbSet<User>().AnyAsync(u => u.UserId == UserId);

        public async Task<User> FindByIdAsync(string id) =>
            await this.GetDbSet<User>().FirstOrDefaultAsync(u => u.UserId == id);

        public async Task<Role> FindRoleByIdAsync(string id)
            => await this.GetDbSet<Role>().FirstOrDefaultAsync(r => r.RoleId == id);

        public async Task<List<Role>> GetAllRolesAsync() =>
            await this.GetDbSet<Role>().ToListAsync();
        #endregion
    }

}
