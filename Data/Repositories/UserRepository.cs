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
            await this.GetDbSet<User>().Where(u => !u.IsDeleted).Include(u => u.Role).ToListAsync();

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

        public void AddAdmin(Admin nlo)
        {
            this.GetDbSet<Admin>().Add(nlo);
            UnitOfWork.SaveChanges();
        }

        public async Task UpdateUserAsync(User user)
        {
            this.GetDbSet<User>().Update(user);
            await UnitOfWork.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(string userId)
        {
            var userToDelete = await this.GetDbSet<User>().Include(u => u.Avatar).FirstOrDefaultAsync(u => u.UserId == userId);
            if (userToDelete != null)
            {
                if (userToDelete.Avatar != null)
                {
                    this.GetDbSet<Avatar>().Remove(userToDelete.Avatar);
                }
                userToDelete.IsDeleted = true;
                this.GetDbSet<User>().Update(userToDelete);
                await UnitOfWork.SaveChangesAsync();
            }
        }

        public async Task DeleteAdminAsync(string AdminId)
        {
            var adminToDelete = await this.GetDbSet<Admin>().FirstOrDefaultAsync(a => a.UserId == AdminId);
            if (adminToDelete != null)
            {
                this.GetDbSet<Admin>().Remove(adminToDelete);
                // changes saved with the next call to SaveChanges
            }
        }

        public async Task DeleteApplicantAsync(string applicantId)
        {
            var applicantToDelete = await this.GetDbSet<Applicant>().Include(a => a.Resume).FirstOrDefaultAsync(a => a.UserId == applicantId);
            if (applicantToDelete != null)
            {
                if (applicantToDelete.Resume != null)
                {
                    this.GetDbSet<Resume>().Remove(applicantToDelete.Resume);
                }
                this.GetDbSet<Applicant>().Remove(applicantToDelete);
                // changes saved with the next call to SaveChanges
            }
        }

        public async Task DeleteRecruiterAsync(string recruiterId)
        {
            var recruiterToDelete = await this.GetDbSet<Recruiter>().FirstOrDefaultAsync(r => r.UserId == recruiterId);
            if (recruiterToDelete != null)
            {
                this.GetDbSet<Recruiter>().Remove(recruiterToDelete);
                // changes saved with the next call to SaveChanges
            }
        }

        #region Helper Methods
        public async Task<User> FindUserByIdAsync(string id) =>
            await this.GetDbSet<User>().FirstOrDefaultAsync(u => u.UserId == id);

        public async Task<Role> FindRoleByIdAsync(string id)
            => await this.GetDbSet<Role>().FirstOrDefaultAsync(r => r.RoleId == id);

        public async Task<List<Role>> GetAllRolesAsync() =>
            await this.GetDbSet<Role>().ToListAsync();
        #endregion
    }

}
