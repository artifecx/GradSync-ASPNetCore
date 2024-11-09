using Data.Models;
using Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IJobService
    {
        Task AddJobAsync(JobViewModel model);
        Task UpdateJobAsync(JobViewModel model);
        Task ArchiveJobAsync(string id);
        Task UnarchiveJobAsync(JobServiceModel model);
        Task<List<JobViewModel>> GetAllJobsAsync();
        Task<PaginatedList<JobViewModel>> GetAllJobsAsync(FilterServiceModel filters, string archived = null);
        Task<PaginatedList<JobViewModel>> GetRecruiterJobsAsync(FilterServiceModel filters, string archived = null);
        Task<JobViewModel> GetJobByIdAsync(string id);
    }
}
