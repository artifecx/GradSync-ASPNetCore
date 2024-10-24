using Data.Interfaces;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Resources.Constants.UserRoles;

namespace Data.Repositories
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserRepository"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work.</param>
        public UserRepository(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Get Methods        
        /// <summary>
        /// Retrieves undeleted users with the necessary navigation properties.
        /// Includes: 
        /// Role
        /// </summary>
        /// <returns>A <see cref="List{T}"/> of <see cref="User"/> including the specified related data.</returns>
        public async Task<List<User>> GetAllUsersAsync() =>
            await this.GetDbSet<User>().Where(u => !u.IsDeleted).Include(u => u.Role).ToListAsync();

        /// <summary>
        /// Retrieves all users including deleted ones as untracked with no navigation properties set.
        /// </summary>
        /// <returns>A <see cref="List{T}"/> of untracked <see cref="User"/> with no navigation properties set.</returns>
        public async Task<List<User>> GetAllUsersNoIncludesAsync() =>
            await this.GetDbSet<User>().AsNoTracking().ToListAsync();

        /// <summary>
        /// Retrieves a user with no navigation properties.
        /// </summary>
        /// <returns>A <see cref="User"/> with no related data.</returns>
        public async Task<User> GetUserByIdAsync(string id) =>
            await this.GetDbSet<User>().FirstOrDefaultAsync(u => u.UserId == id);

        /// <summary>
        /// Retrieves a user with no navigation properties.
        /// </summary>
        /// <returns>A <see cref="User"/> with no related data.</returns>
        public async Task<User> GetUserByEmailAsync(string email) =>
            await this.GetDbSet<User>().FirstOrDefaultAsync(u => u.Email == email);

        /// <summary>
        /// Retrieves a user with no navigation properties.
        /// </summary>
        /// <returns>A <see cref="User"/> with no related data.</returns>
        public async Task<User> GetUserByEmailAndPasswordAsync(string email, string passwordKey) =>
            await this.GetDbSet<User>().FirstOrDefaultAsync(u => u.Email == email && u.Password == passwordKey);

        /// <summary>
        /// Retrieves a user using the provided token.
        /// </summary>
        /// <returns>A <see cref="User"/> with no related data.</returns>
        public async Task<User> GetUserByTokenAsync(string token) =>
            await this.GetDbSet<User>().FirstOrDefaultAsync(u => u.Token == token);

        /// <summary>
        /// Retrieves all roles.
        /// </summary>
        /// <returns>A <see cref="List{T}"/> of <see cref="Role"/></returns>
        public async Task<List<Role>> GetAllRolesAsync() =>
            await this.GetDbSet<Role>().ToListAsync();
        #endregion

        #region User CRUD Methods
        /// <summary>
        /// Adds a new user.
        /// </summary>
        /// <param name="user">The user to add.</param>
        /// <returns><see cref="void"/></returns>
        public void AddUser(User user)
        {
            this.GetDbSet<User>().Add(user);
            UnitOfWork.SaveChanges();
        }

        /// <summary>
        /// Updates the user asynchronously.
        /// </summary>
        /// <param name="user">The user to update.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task UpdateUserAsync(User user)
        {
            this.GetDbSet<User>().Update(user);
            await UnitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// Soft deletes the user asynchronously.
        /// Will hard delete the user's avatar if it exists 
        /// and their corresponding role entity: <see cref="Admin"/>, <see cref="Applicant"/>, <see cref="Recruiter"/>.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
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

            if (string.Equals(ids.RoleId, Role_Admin))
                await DeleteAdminAsync(ids.UserId);
            else if (string.Equals(ids.RoleId, Role_Applicant))
                await DeleteApplicantAsync(ids.UserId);
            else if (string.Equals(ids.RoleId, Role_Recruiter))
                await DeleteRecruiterAsync(ids.UserId);

            await GetDbSet<User>()
                .Where(u => u.UserId == userId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(u => u.IsDeleted, true)
                    .SetProperty(u => u.AvatarId, (string)null));
        }


        #endregion

        #region Admin CRUD Methods
        /// <summary>
        /// Adds a new admin. Only called when creating a new <see cref="User"/> with the role of <see cref="Admin"/>.
        /// </summary>
        /// <param name="admin">The admin to add.</param>
        /// <returns><see cref="void"/></returns>
        public void AddAdmin(Admin admin)
        {
            this.GetDbSet<Admin>().Add(admin);
            UnitOfWork.SaveChanges();
        }

        /// <summary>
        /// Performs a hard delete of an <see cref="Admin"/> asynchronously. 
        /// Only called when deleting a <see cref="User"/> with an administrator role.
        /// </summary>
        /// <param name="adminId">The admin identifier.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task DeleteAdminAsync(string adminId) =>
            await GetDbSet<Admin>()
                .Where(a => a.UserId == adminId)
                .ExecuteDeleteAsync();
        #endregion

        #region Applicant CRUD Methods
        /// <summary>
        /// Adds a new applicant. 
        /// Only called when creating a new <see cref="User"/> with the role of <see cref="Applicant"/>.
        /// </summary>
        /// <param name="applicant">The applicant to add.</param>
        /// <returns><see cref="void"/></returns>
        public void AddApplicant(Applicant applicant)
        {
            this.GetDbSet<Applicant>().Add(applicant);
            UnitOfWork.SaveChanges();
        }

        /// <summary>
        /// Performs a hard delete of an <see cref="Applicant"/> asynchronously. 
        /// Only called when deleting a <see cref="User"/> with an applicant role.
        /// </summary>
        /// <param name="applicantId">The applicant identifier.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task DeleteApplicantAsync(string applicantId) =>
            await GetDbSet<Applicant>()
                .Where(a => a.UserId == applicantId)
                .ExecuteDeleteAsync();
        #endregion

        #region Recruiter CRUD Methods
        /// <summary>
        /// Adds a new recruiter. 
        /// Only called when creating a new <see cref="User"/> with the role of <see cref="Recruiter"/>.
        /// </summary>
        /// <param name="recruiter">The recruiter to add.</param>
        /// <returns><see cref="void"/></returns>
        public void AddRecruiter(Recruiter recruiter)
        {
            this.GetDbSet<Recruiter>().Add(recruiter);
            UnitOfWork.SaveChanges();
        }

        /// <summary>
        /// Performs a hard delete of a <see cref="Recruiter"/> asynchronously. 
        /// Only called when deleting a <see cref="User"/> with a recruiter role.
        /// </summary>
        /// <param name="recruiterId">The recruiter identifier.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task DeleteRecruiterAsync(string recruiterId) =>
            await GetDbSet<Recruiter>()
                .Where(a => a.UserId == recruiterId)
                .ExecuteDeleteAsync();
        #endregion

        /// <summary>
        /// Determines whether the logged in <see cref="Admin"/> is a super admin.
        /// </summary>
        /// <param name="adminId">The admin identifier.</param>
        /// <returns>
        ///   <c>true</c> if [is super admin]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsSuperAdmin(string adminId)
        {
            return this.GetDbSet<Admin>().Any(a => a.UserId == adminId && a.IsSuper == true);
        }

    }
}
