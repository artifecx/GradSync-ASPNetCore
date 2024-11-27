using Data.Dtos;
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
        Task UpdateJobStatusAsync(string jobId, string statusId);
        Task ArchiveJobAsync(string id);
        Task UnarchiveJobAsync(JobServiceModel model);
        Task IncrementJobSlots(string jobId);
        Task DecrementJobSlots(string jobId);
        Task<List<FeaturedJobsViewModel>> GetApplicantFeaturedJobsAsync(string userId);
        Task<ApplicantViewDto> GetApplicantDetailsAsync(string applicantId);
        Task<PaginatedList<JobViewModel>> GetAllJobsAsync(FilterServiceModel filters, string archived = null);
        Task<JobViewModel> GetJobByIdAsync(string id);
    }
}
