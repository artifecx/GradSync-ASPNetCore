using Data.Models;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data.Interfaces
{
    public interface IApplicationRepository
    {
        Task AddApplicationAsync(Application application);
        Task UpdateApplicationAsync(Application application);
        Task DeleteApplicationAsync(Application application);
        Task<List<Job>> GetAllJobsAsync();
        IQueryable<Application> GetApplicationWithIncludes();
        

        //{
        //    return UnitOfWork.Context.Applications
        //        .Include(a => a.Job)
        //        .Include(a => a.ApplicationStatusType)
        //        .AsQueryable();
        //}
    }
}
