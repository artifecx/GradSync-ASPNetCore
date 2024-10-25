using Data.Interfaces;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Data.Repositories
{
    /// <summary>
    /// Repository class for handling operations related to user profiles.
    /// </summary>
    public class UserProfileRepository : BaseRepository, IUserProfileRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserProfileRepository"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work.</param>
        public UserProfileRepository(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        /// <summary>
        /// Retrieves the user profiles for a specified user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>A dictionary containing the user profiles.</returns>
        public Dictionary<string, string> GetUserProfile(string userId)
        {
            var preferences = this.GetDbSet<User>()
                                  .Where(x => x.UserId == userId)
                                  .Select(x => x.Preferences)
                                  .FirstOrDefault();
            if (preferences == null)
            {
                return new Dictionary<string, string>();
            }
            return JsonSerializer.Deserialize<Dictionary<string, string>>(preferences);
        }

        /// <summary>
        /// Updates the user profiles asynchronously.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="updatedPreferences">The updated preferences.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task UpdateUserProfileAsync(string userId, Dictionary<string, string> updatedPreferences)
        {
            var user = await this.GetDbSet<User>().FirstOrDefaultAsync(x => x.UserId == userId);
            if (user != null)
            {
                user.Preferences = JsonSerializer.Serialize(updatedPreferences);
                this.GetDbSet<User>().Update(user);
                await UnitOfWork.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Finds a specific user preference by key asynchronously.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="key">The preference key.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation. 
        /// The task result contains a key-value pair representing the preference.</returns>
        public async Task<KeyValuePair<string, string>> GetUserPreferenceByKeyAsync(string userId, string key) =>
            GetUserProfile(userId).FirstOrDefault(x => x.Key == key);

        public async Task<User> GetApplicantProfileByUserId(string userId)
        {
            var applicant = await this.GetDbSet<User>()
                .Where(u => !u.IsDeleted && u.UserId == userId)
                .Include(u => u.Applicant)
                .Select(u => new User
                {
                    UserId = u.UserId,
                    FirstName = u.FirstName,
                    MiddleName = u.MiddleName,
                    LastName = u.LastName,
                    Suffix = u.Suffix,
                    Email = u.Email,
                    JoinDate = u.JoinDate,
                    LastLoginDate = u.LastLoginDate,
                    Applicant = new Applicant
                    {
                        UserId = u.Applicant.UserId,
                        ResumeId = u.Applicant.ResumeId,
                        EducationalDetailId = u.Applicant.EducationalDetailId,
                        JobPreferences = u.Applicant.JobPreferences,
                        Address = u.Applicant.Address,
                    }
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();

            var educationalDetail = new EducationalDetail();
            if (applicant?.Applicant != null)
            {
                educationalDetail = await this.GetDbSet<EducationalDetail>()
                    .Where(ed => ed.Applicants.Select(a => a.UserId == applicant.UserId).FirstOrDefault())
                    .Include(ed => ed.YearLevel)
                    .Include(ed => ed.College)
                    .Include(ed => ed.Department)
                    .Include(ed => ed.Program)
                    .Select(ed => new EducationalDetail
                    {
                        EducationalDetailId = ed.EducationalDetailId,
                        IdNumber = ed.IdNumber,
                        ProgramId = ed.ProgramId,
                        DepartmentId = ed.DepartmentId,
                        CollegeId = ed.CollegeId,
                        YearLevelId = ed.YearLevelId,
                        IsGraduate = ed.IsGraduate,
                        College = new College
                        {
                            CollegeId = ed.College.CollegeId,
                            Name = ed.College.Name,
                            ShortName = ed.College.ShortName
                        },
                        Department = new Department
                        {
                            DepartmentId = ed.Department.DepartmentId,
                            Name = ed.Department.Name,
                            ShortName = ed.Department.ShortName,
                            CollegeId = ed.Department.CollegeId
                        },
                        Program = new Program
                        {
                            ProgramId = ed.Program.ProgramId,
                            Name = ed.Program.Name,
                            ShortName = ed.Program.ShortName,
                            DepartmentId = ed.Program.DepartmentId
                        },
                        YearLevel = ed.YearLevel
                    })
                    .AsNoTracking()
                    .FirstOrDefaultAsync();
            }
            applicant.Applicant.EducationalDetail = educationalDetail;
            return applicant;
        }

        public async Task<User> GetRecruiterProfileByUserId(string userId)
        {
            return await this.GetDbSet<User>()
                .Where(u => !u.IsDeleted && u.UserId == userId)
                .Include(u => u.Recruiter)
                    .ThenInclude(r => r.Company)
                .Select(u => new User
                {
                    UserId = u.UserId,
                    FirstName = u.FirstName,
                    MiddleName = u.MiddleName,
                    LastName = u.LastName,
                    Suffix = u.Suffix,
                    Email = u.Email,
                    JoinDate = u.JoinDate,
                    LastLoginDate = u.LastLoginDate,
                    Recruiter = new Recruiter
                    {
                        UserId = u.Recruiter.UserId,
                        Title = u.Recruiter.Title,
                        CompanyId = u.Recruiter.CompanyId,
                        Company = new Company
                        {
                            CompanyId = u.Recruiter.Company.CompanyId,
                            Name = u.Recruiter.Company.Name,
                            Address = u.Recruiter.Company.Address,
                            ContactEmail = u.Recruiter.Company.ContactEmail,
                            ContactNumber = u.Recruiter.Company.ContactNumber,
                        }
                    }
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }
    }
}
