using Data.Interfaces;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
                            .AsNoTracking()
                        .Include(j => j.YearLevel)
                            .AsNoTracking()
                        .Include(j => j.JobDepartments)
                            .ThenInclude(j => j.Department)
                            .AsNoTracking()
                        .Include(j => j.PostedBy)
                            .ThenInclude(r => r.Company)
                            .AsNoTracking()
                        .Include(j => j.PostedBy)
                            .ThenInclude(r => r.User)
                            .AsNoTracking()
                        .Include(j => j.EmploymentType)
                            .AsNoTracking()
                        .Include(j => j.SetupType)
                            .AsNoTracking()
                        .Include(j => j.StatusType)
                            .AsNoTracking();
        }

        public async Task<List<Application>> GetAllApplicationsNoIncludesAsync() =>
            await this.GetDbSet<Application>().AsNoTracking().ToListAsync();

        public async Task<List<Job>> GetAllJobsDepartmentsIncludeAsync() =>
            await this.GetDbSet<Job>().Include(j => j.JobDepartments).ThenInclude(j => j.Department).AsNoTracking().ToListAsync();

        public async Task<List<Job>> GetAllJobsAsync() =>
            await GetJobsWithIncludes().AsNoTracking().ToListAsync();

        public async Task<List<Job>> GetRecruiterJobsAsync(string userId) =>
            await GetJobsWithIncludes().Where(j => string.Equals(j.PostedById, userId)).AsNoTracking().ToListAsync();

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

        public async Task DeleteJobAsync(Job job)
        {
            job.IsArchived = true;
            this.GetDbSet<Job>().Update(job);
            await UnitOfWork.SaveChangesAsync();
        }

        public async Task<Job> GetJobByIdAsync(string id) =>
            await GetJobsWithIncludes().AsNoTracking().FirstOrDefaultAsync(j => j.JobId == id);

        public async Task<List<Company>> GetCompaniesWithListingsAsync()
        {
            var companies = await this.GetDbSet<Company>()
                .Include(c => c.Recruiters)
                .ThenInclude(r => r.Jobs).AsNoTracking()
                .Where(Company => Company.Recruiters.Any(r => r.Jobs.Any()))
                .AsNoTracking()
                .ToListAsync();

            return companies;
        }

        public async Task<List<EmploymentType>> GetEmploymentTypesAsync() =>
            await this.GetDbSet<EmploymentType>().AsNoTracking().ToListAsync();

        public async Task<List<StatusType>> GetStatusTypesAsync() =>
            await this.GetDbSet<StatusType>().AsNoTracking().ToListAsync();

        public async Task<List<SetupType>> GetWorkSetupsAsync() =>
            await this.GetDbSet<SetupType>().AsNoTracking().ToListAsync();

        public async Task<List<YearLevel>> GetYearLevelsAsync() =>
            await this.GetDbSet<YearLevel>().AsNoTracking().ToListAsync();

        public async Task<List<Department>> GetDepartmentsAsync() =>
            await this.GetDbSet<Department>().AsNoTracking().Include(d => d.College).ToListAsync();

        public async Task<List<Skill>> GetSkillsAsync() =>
            await this.GetDbSet<Skill>().AsNoTracking().ToListAsync();
    }
}
