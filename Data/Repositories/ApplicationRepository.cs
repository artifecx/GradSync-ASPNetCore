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
    public class ApplicationRepository : BaseRepository, IApplicationRepository
    {
        public ApplicationRepository(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        public async Task AddApplicationAsync(Application application)
        {

        }
        public async Task UpdateApplicationAsync(Application application)
        {

        }
        public async Task DeleteApplicationAsync(Application application)
        {

        }
        public async Task<List<Job>> GetAllJobsAsync()
        {
            return new List<Job>();
        }
        public IQueryable<Application> GetApplicationWithIncludes()
        {
            return this.GetDbSet<Application>()           
                .Include(a => a.Job)
                .Include(a => a.ApplicationStatusType)
                .Where(a => !a.IsArchived)
                .AsQueryable();
        }
    }
}
