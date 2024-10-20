using Data.Interfaces;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class JobRepository : BaseRepository, IJobRepository
    {
        public JobRepository(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        private IQueryable<Job> GetJobsWithIncludes()
        {
            return this.GetDbSet<Job>()
                        .Where(j => !j.IsArchived)
                        .Include(j => j.JobSkills)
                            .ThenInclude(j => j.Skill)
                        .Include(j => j.YearLevel)
                        .Include(j => j.JobPrograms)
                            .ThenInclude(j => j.Program)
                        .Include(j => j.PostedBy)
                            .ThenInclude(r => r.Company)
                        .Include(j => j.PostedBy)
                            .ThenInclude(r => r.User)
                        .Include(j => j.EmploymentType)
                        .Include(j => j.SetupType)
                        .Include(j => j.StatusType)
                        .Include(j => j.Applications);
        }

        public async Task<List<Application>> GetAllApplicationsNoIncludesAsync() =>
            await this.GetDbSet<Application>().AsNoTracking().ToListAsync();

        public async Task<List<Job>> GetAllJobsProgramsIncludeAsync() =>
            await this.GetDbSet<Job>().Include(j => j.JobPrograms).ThenInclude(j => j.Program).AsNoTracking().ToListAsync();

        public async Task<List<Job>> GetAllJobsAsync() =>
            await GetJobsWithIncludes().AsNoTracking().ToListAsync();

        public async Task<List<Job>> GetArchivedJobsAsync() =>
            await this.GetDbSet<Job>().Where(j => j.IsArchived).ToListAsync();

        public async Task<List<Job>> GetRecruiterJobsAsync(string userId) =>
            await GetJobsWithIncludes().Where(j => string.Equals(j.PostedById, userId)).AsNoTracking().ToListAsync();

        public async Task<List<Job>> GetRecruiterArchivedJobsAsync(string userId) =>
            await this.GetDbSet<Job>().Where(j => j.IsArchived && string.Equals(j.PostedById, userId)).ToListAsync();

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

        public async Task<Job> GetJobByIdAsync(string id, bool? track)
        {
            var query = track.GetValueOrDefault() ? GetJobsWithIncludes() : GetJobsWithIncludes().AsNoTracking();
            return await query.FirstOrDefaultAsync(j => j.JobId == id);
        }

        public async Task<Job> GetJobByIdAsync(string id, string isArchived) =>
            await this.GetDbSet<Job>().FirstOrDefaultAsync(j => j.IsArchived == Convert.ToBoolean(isArchived) && j.JobId == id);

        public async Task<List<EmploymentType>> GetEmploymentTypesAsync() =>
            await this.GetDbSet<EmploymentType>().AsNoTracking().ToListAsync();

        public async Task<List<StatusType>> GetStatusTypesAsync() =>
            await this.GetDbSet<StatusType>().AsNoTracking().ToListAsync();

        public async Task<List<SetupType>> GetWorkSetupsAsync() =>
            await this.GetDbSet<SetupType>().AsNoTracking().ToListAsync();

        public async Task<List<YearLevel>> GetYearLevelsAsync() =>
            await this.GetDbSet<YearLevel>().AsNoTracking().ToListAsync();

        public async Task<List<Program>> GetProgramsAsync() =>
            await this.GetDbSet<Program>()
                .Where(p => !p.IsDeleted)
                .Include(p => p.Department)
                .ThenInclude(d => d.College)
                .AsNoTracking()
                .ToListAsync();

        public async Task<List<Skill>> GetSkillsAsync() =>
            await this.GetDbSet<Skill>().AsNoTracking().ToListAsync();

        public bool HasChanges(Job job)
        {
            var entry = this.GetDbSet<Job>().Entry(job);
            return entry.Properties.Any(p => p.IsModified);
        }
    }
}
