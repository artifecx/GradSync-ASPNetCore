using Data.Dtos;
using Data.Interfaces;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class JobRepository : BaseRepository, IJobRepository
    {
        public JobRepository(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        public async Task<List<Job>> GetJobsAsync(ISpecification<Job> specification, bool track)
        {
            var query = ApplySpecification(specification);
            return await (track ? query.ToListAsync() : query.AsNoTracking().ToListAsync());    
        }

        public async Task<Job> GetJobAsync(ISpecification<Job> specification, bool track)
        {
            var query = ApplySpecification(specification);
            return await (track ? query : query.AsNoTracking()).FirstOrDefaultAsync();
        }

        public async Task AddJobAsync(Job job)
        {
            await this.GetDbSet<Job>().AddAsync(job);
            await UnitOfWork.SaveChangesAsync();
        }

        public async Task UpdateJobAsync(Job job)
        {
            this.GetDbSet<Job>().Update(job);
            await UnitOfWork.SaveChangesAsync();
        }

        public async Task<List<Application>> GetAllApplicationsNoIncludesAsync() =>
            await this.GetDbSet<Application>().AsNoTracking().ToListAsync();

        public async Task<List<Job>> GetAllJobsProgramsIncludeAsync() =>
            await this.GetDbSet<Job>().Include(j => j.JobPrograms).ThenInclude(j => j.Program).AsNoTracking().ToListAsync();

        public async Task<List<JobApplicantMatch>> GetApplicantFeaturedJobsAsync(string userId)
        {
            return await this.GetDbSet<JobApplicantMatch>()
                .Include(jam => jam.Job)
                    .ThenInclude(j => j.JobSkills)
                        .ThenInclude(js => js.Skill)
                .Include(jam => jam.Job)
                    .ThenInclude(j => j.EmploymentType)
                .Include(jam => jam.Job)
                    .ThenInclude(j => j.SetupType)
                .Include(jam => jam.Job)
                    .ThenInclude(j => j.StatusType)
                .Include(jam => jam.Job)
                    .ThenInclude(j => j.Company)
                .Where(jam => jam.UserId == userId
                    && jam.MatchPercentage >= 70
                    && !jam.Job.IsArchived
                    && jam.Job.StatusTypeId == "Open")
                .Select(jam => new JobApplicantMatch
                {
                    JobId = jam.JobId,
                    MatchPercentage = jam.MatchPercentage,
                    Job = new Job
                    {
                        JobId = jam.Job.JobId,
                        Title = jam.Job.Title,
                        Location = jam.Job.Location,
                        SalaryLower = jam.Job.SalaryLower,
                        SalaryUpper = jam.Job.SalaryUpper,
                        JobSkills = jam.Job.JobSkills,
                        EmploymentType = new EmploymentType
                        {
                            Name = jam.Job.EmploymentType.Name
                        },
                        SetupType = new SetupType
                        {
                            Name = jam.Job.SetupType.Name
                        },
                        StatusType = new StatusType
                        {
                            Name = jam.Job.StatusType.Name
                        },
                        Company = new Company
                        {
                            Name = jam.Job.Company.Name
                        }
                    },
                })
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<ApplicantViewDto> GetApplicantDetailsAsync(string applicantId)
        {
            var applicant = await this.GetDbSet<Applicant>()
                .Where(a => a.UserId == applicantId)
                .Include(a => a.User)
                .Include(a => a.ApplicantSkills)
                    .ThenInclude(a => a.Skill)
                .Include(a => a.EducationalDetail)
                    .ThenInclude(e => e.Program)
                .Include(a => a.EducationalDetail)
                    .ThenInclude(e => e.Department)
                .Include(a => a.EducationalDetail)
                    .ThenInclude(e => e.College)
                .Include(a => a.EducationalDetail)
                    .ThenInclude(e => e.YearLevel)
                .Select(a => new ApplicantViewDto
                {
                    ApplicantId = a.UserId,
                    Name = $"{a.User.FirstName}"
                        + $"{(string.IsNullOrWhiteSpace(a.User.MiddleName) ? "" : " " + a.User.MiddleName)}"
                        + $" {a.User.LastName}"
                        + $"{(string.IsNullOrWhiteSpace(a.User.Suffix) ? "" : " " + a.User.Suffix)}",
                    Email = a.User.Email,
                    TechnicalSkills = a.ApplicantSkills
                        .Where(s => s.Type == "Technical")
                        .Select(s => s.Skill.Name)
                        .ToList(),
                    CulturalSkills = a.ApplicantSkills
                        .Where(s => s.Type == "Cultural")
                        .Select(s => s.Skill.Name)
                        .ToList(),
                    Certifications = a.ApplicantSkills
                        .Where(s => s.Type == "Certification")
                        .Select(s => s.Skill.Name)
                        .ToList(),
                    ProgramName = a.EducationalDetail.Program.Name,
                    DepartmentName = a.EducationalDetail.Department.Name,
                    CollegeName = a.EducationalDetail.College.Name,
                    YearLevelName = a.EducationalDetail.YearLevel.Name,
                    DepartmentId = a.EducationalDetail.DepartmentId
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return applicant;
        }

        public bool HasChanges(Job job)
        {
            var entry = this.GetDbSet<Job>().Entry(job);
            return entry.Properties
                .Any(p => !string.Equals(
                        p.Metadata.Name, 
                        nameof(job.CompanyId), StringComparison.OrdinalIgnoreCase) 
                        && p.IsModified);
        }
    }
}
