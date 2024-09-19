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
                        .Include(j => j.Skills)
                        .Include(j => j.YearLevel)
                        .Include(j => j.Departments)
                        .Include(j => j.PostedBy)
                            .ThenInclude(r => r.Company)
                        .Include(j => j.EmploymentType)
                        .Include(j => j.Schedule)
                        .Include(j => j.SetupType)
                        .Include(j => j.StatusType);
        }

        public async Task<List<Job>> GetAllJobsAsync() =>
            await GetJobsWithIncludes().AsNoTracking().ToListAsync();

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

        public async Task<Job> FindJobByIdAsync(string id) =>
            await GetJobsWithIncludes().FirstOrDefaultAsync(j => j.JobId == id);

        public async Task<List<Company>> GetCompaniesWithListingsAsync()
        {
            var companies = await this.GetDbSet<Company>()
                .Include(c => c.Recruiters)
                .ThenInclude(r => r.Jobs)
                .Where(Company => Company.Recruiters.Any(r => r.Jobs.Any()))
                .AsNoTracking()
                .ToListAsync();

            return companies;
        }

        public async Task<List<EmploymentType>> GetEmploymentTypesAsync() =>
            await this.GetDbSet<EmploymentType>().ToListAsync();

        public async Task<List<StatusType>> GetStatusTypesAsync() =>
            await this.GetDbSet<StatusType>().ToListAsync();

        public async Task<List<SetupType>> GetWorkSetupsAsync() =>
            await this.GetDbSet<SetupType>().ToListAsync();

    }
}
