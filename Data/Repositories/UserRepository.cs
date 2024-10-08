﻿using Data.Interfaces;
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

        public async Task<List<User>> GetAllUsersNoIncludesAsync() =>
            await this.GetDbSet<User>().AsNoTracking().ToListAsync();

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
            var ids = await GetDbSet<User>()
                .Where(u => u.UserId == userId)
                .Select(u => new User
                {
                    UserId = u.UserId,
                    AvatarId = u.AvatarId,
                    RoleId = u.RoleId
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (ids.AvatarId != null)
            {
                await GetDbSet<Avatar>()
                    .Where(a => a.AvatarId == ids.AvatarId)
                    .ExecuteDeleteAsync();
            }

            if (string.Equals(ids.RoleId, "Admin"))
                await DeleteAdminAsync(ids.UserId);
            else if (string.Equals(ids.RoleId, "Applicant"))
                await DeleteApplicantAsync(ids.UserId);
            else if (string.Equals(ids.RoleId, "Recruiter"))
                await DeleteRecruiterAsync(ids.UserId);

            await GetDbSet<User>()
                .Where(u => u.UserId == userId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(u => u.IsDeleted, true)
                    .SetProperty(u => u.AvatarId, (string)null));
        }

        public async Task DeleteAdminAsync(string adminId) =>
            await GetDbSet<Admin>()
                .Where(a => a.UserId == adminId)
                .ExecuteDeleteAsync();

        public async Task DeleteApplicantAsync(string applicantId) =>
            await GetDbSet<Applicant>()
                .Where(a => a.UserId == applicantId)
                .ExecuteDeleteAsync();

        public async Task DeleteRecruiterAsync(string recruiterId) =>
            await GetDbSet<Recruiter>()
                .Where(a => a.UserId == recruiterId)
                .ExecuteDeleteAsync();

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
