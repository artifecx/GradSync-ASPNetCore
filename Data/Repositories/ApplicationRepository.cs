using Data.Interfaces;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data.Repositories
{
    /// <summary>
    /// Repository for handling job application data
    /// </summary>
    public class ApplicationRepository : BaseRepository, IApplicationRepository
    {
        public ApplicationRepository(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        /// <summary>
        /// Gets the applications with selected includes.
        /// </summary>
        /// Application -> ApplicationId, JobId, UserId, ApplicationStatusTypeId, CreatedDate, UpdatedDate, ApplicationStatusType, Job, User
        /// Application.StatusType -> ApplicationStatusTypeId, Name
        /// Application.Job -> JobId, Title, PostedById, CompanyId, Location
        /// Application.Job.Company -> CompanyId, Name, ContactEmail, ContactNumber, Address
        /// Application.Job.PostedBy -> UserId, User
        /// Application.Job.PostedBy.User -> UserId, FirstName, MiddleName, LastName, Suffix, Email
        /// Application.Job.SetupType -> SetupTypeId, Name
        /// Application.Job.EmploymentType -> EmploymentTypeId, Name
        /// Application.User -> UserId, Address, EducationalDetail
        /// Application.User.EducationalDetail -> EducationalDetailId, IdNumber, ProgramId, DepartmentId, CollegeId
        /// Application.User.EducationalDetail.Program -> ProgramId, Name, ShortName
        /// Application.User.EducationalDetail.Department -> DepartmentId, Name, ShortName
        /// Application.User.EducationalDetail.College -> CollegeId, Name, ShortName
        /// Application.User.User -> UserId, FirstName, MiddleName, LastName, Suffix, Email
        /// Application.User.Resume -> ResumeId, FileName, UploadedDate
        /// <returns></returns>
        private IQueryable<Application> GetApplicationsWithIncludes()
        {
            return this.GetDbSet<Application>()
                .Where(a => !a.IsArchived)
                .Include(a => a.Job)
                    .ThenInclude(j => j.SetupType)
                .Include(a => a.Job)
                    .ThenInclude(j => j.EmploymentType)
                .Include(a => a.Job)
                    .ThenInclude(j => j.Company)
                .Include(a => a.Job)
                    .ThenInclude(j => j.PostedBy)
                        .ThenInclude(r => r.User)
                .Include(a => a.User)
                    .ThenInclude(u => u.EducationalDetail)
                        .ThenInclude(ed => ed.Department)
                .Include(a => a.User)
                    .ThenInclude(u => u.EducationalDetail)
                        .ThenInclude(ed => ed.College)
                .Include(a => a.User)
                    .ThenInclude(u => u.EducationalDetail)
                        .ThenInclude(ed => ed.Program)
                .Include(a => a.User)
                    .ThenInclude(u => u.User)
                .Include(a => a.User)
                    .ThenInclude(u => u.Resume)
                .Include(a => a.ApplicationStatusType)
                .Select(a => new Application
                {
                     ApplicationId = a.ApplicationId,
                     JobId = a.JobId,
                     UserId = a.UserId,
                     ApplicationStatusTypeId = a.ApplicationStatusTypeId,
                     CreatedDate = a.CreatedDate,
                     UpdatedDate = a.UpdatedDate,
                     ApplicationStatusType = new ApplicationStatusType
                     {
                         ApplicationStatusTypeId = a.ApplicationStatusType.ApplicationStatusTypeId,
                         Name = a.ApplicationStatusType.Name,
                     },
                     Job = new Job
                     {
                         JobId = a.Job.JobId,
                         Title = a.Job.Title,
                         PostedById = a.Job.PostedById,
                         CompanyId = a.Job.CompanyId,
                         Location = a.Job.Location,
                         Company = new Company
                         {
                             CompanyId = a.Job.Company.CompanyId,
                             Name = a.Job.Company.Name,
                             ContactEmail = a.Job.Company.ContactEmail,
                             ContactNumber = a.Job.Company.ContactNumber,
                             Address = a.Job.Company.Address,
                         },
                         PostedBy = new Recruiter
                         {
                             UserId = a.Job.PostedBy.UserId,  
                             Title = a.Job.PostedBy.Title,
                             User = new User
                             {
                                 UserId = a.Job.PostedBy.User.UserId,
                                 FirstName = a.Job.PostedBy.User.FirstName,
                                 MiddleName = a.Job.PostedBy.User.MiddleName,
                                 LastName = a.Job.PostedBy.User.LastName,
                                 Suffix = a.Job.PostedBy.User.Suffix,
                                 Email = a.Job.PostedBy.User.Email,
                             },
                         },
                         SetupType = new SetupType
                         {
                             SetupTypeId = a.Job.SetupType.SetupTypeId,
                             Name = a.Job.SetupType.Name
                         },
                         EmploymentType = new EmploymentType
                         {
                             EmploymentTypeId = a.Job.EmploymentType.EmploymentTypeId,
                             Name = a.Job.EmploymentType.Name
                         },
                     },
                     User = new Applicant
                     {
                         UserId = a.User.UserId,
                         Address = a.User.Address,
                         EducationalDetail = new EducationalDetail
                         {
                             EducationalDetailId = a.User.EducationalDetail.EducationalDetailId,
                             IdNumber = a.User.EducationalDetail.IdNumber,
                             ProgramId = a.User.EducationalDetail.ProgramId,
                             DepartmentId = a.User.EducationalDetail.DepartmentId,
                             CollegeId = a.User.EducationalDetail.CollegeId,
                             Program = new Program
                             {
                                 ProgramId = a.User.EducationalDetail.Program.ProgramId,
                                 Name = a.User.EducationalDetail.Program.Name,
                                 ShortName = a.User.EducationalDetail.Program.ShortName,
                             },
                             Department = new Department
                             {
                                 DepartmentId = a.User.EducationalDetail.Department.DepartmentId,
                                 Name = a.User.EducationalDetail.Department.Name,
                                 ShortName = a.User.EducationalDetail.Department.ShortName,
                             },
                             College = new College
                             {
                                 CollegeId = a.User.EducationalDetail.College.CollegeId,
                                 Name = a.User.EducationalDetail.College.Name,
                                 ShortName = a.User.EducationalDetail.College.ShortName
                             }
                         },
                         User = new User
                         {
                             UserId = a.User.User.UserId,
                             FirstName = a.User.User.FirstName,
                             MiddleName = a.User.User.MiddleName,
                             LastName = a.User.User.LastName,
                             Suffix = a.User.User.Suffix,
                             Email = a.User.User.Email,
                         },
                         Resume = new Resume
                         {
                             ResumeId = a.User.Resume.ResumeId,
                             FileName = a.User.Resume.FileName,
                             UploadedDate = a.User.Resume.UploadedDate,
                         },
                     },
                 }).AsNoTracking();
        }

        /// <summary>
        /// Asynchronously adds a new application to the database.
        /// </summary>
        /// <param name="application">The application to add.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task AddApplicationAsync(Application application)
        {
            this.GetDbSet<Application>().Add(application);
            await this.UnitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// Asynchronously updates an existing application in the database.
        /// </summary>
        /// <param name="application">The application to update.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task UpdateApplicationAsync(Application application)
        {
            this.GetDbSet<Application>().Update(application);
            await this.UnitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// Retrieves all applications from the database, optionally including related entities.
        /// </summary>
        /// <param name="includes">Indicates whether to include related entities in the result.</param>
        /// <returns>A task representing the asynchronous operation, containing a list of applications.</returns>
        public async Task<List<Application>> GetAllApplicationsAsync(bool includes)
        {
            var query = includes ? 
                GetApplicationsWithIncludes() : 
                this.GetDbSet<Application>()
                    .Where(a => !a.IsArchived)
                    .AsNoTracking();
            return await query.ToListAsync();
        }

        /// <summary>
        /// Retrieves a specific application by its ID from the database.
        /// </summary>
        /// <param name="id">The ID of the application to retrieve.</param>
        /// <returns>A task representing the asynchronous operation, containing the application if found; otherwise, null.</returns>
        public async Task<Application> GetApplicationByIdAsync(string id) =>
            await GetApplicationsWithIncludes().AsNoTracking().FirstOrDefaultAsync(a => a.ApplicationId == id);

        /// <summary>
        /// Retrieves all applications associated with a specific user from the database.
        /// </summary>
        /// <param name="userId">The ID of the user whose applications to retrieve.</param>
        /// <returns>A task representing the asynchronous operation, containing a list of applications.</returns>
        public async Task<List<Application>> GetAllApplicationsByUserAsync(string userId) =>
            await this.GetDbSet<Application>().Where(a => a.UserId == userId).AsNoTracking().ToListAsync();
    }
}
