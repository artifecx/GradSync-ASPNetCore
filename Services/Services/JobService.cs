using Data.Interfaces;
using Data.Models;
using Services.Interfaces;
using Services.ServiceModels;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Resources.Messages;

namespace Services.Services
{
    /// <summary>
    /// Service class for handling operations related to teams.
    /// </summary>
    public class JobService : IJobService
    {
        private readonly IJobRepository _repository;
        private readonly IMapper _mapper;

        public JobService(
            IJobRepository repository,
            IMapper mapper,
            ILogger<JobService> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task AddJobAsync(JobViewModel model)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateJobAsync(JobViewModel model)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteJobAsync(string id)
        {
            throw new NotImplementedException();
        }

        #region Get Methods        
        public async Task<PaginatedList<JobViewModel>> GetAllJobsAsync(
            string sortBy, string search, string filterByCompany,
            string filterByEmploymentType, string filterByStatusType,
            string filterByWorkSetup, int pageIndex, int pageSize)
        {
            var jobs = _mapper.Map<List<JobViewModel>>(await _repository.GetAllJobsAsync());

            if (!string.IsNullOrEmpty(search))
            {
                // TODO: Add more search filters
                jobs = jobs.Where(job => job.Title.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if(!string.IsNullOrEmpty(filterByCompany))
            {
                jobs = jobs.Where(job => job.Company.Name == filterByCompany).ToList();
            }

            if (!string.IsNullOrEmpty(filterByEmploymentType))
            {
                jobs = jobs.Where(job => job.EmploymentType.Name == filterByEmploymentType).ToList();
            }

            if (!string.IsNullOrEmpty(filterByStatusType))
            {
                jobs = jobs.Where(job => job.StatusType.Name == filterByStatusType).ToList();
            }

            if (!string.IsNullOrEmpty(filterByWorkSetup))
            {
                jobs = jobs.Where(job => job.SetupType.Name == filterByWorkSetup).ToList();
            }

            jobs = sortBy switch
            {
                "created_desc" => jobs.OrderByDescending(j => j.CreatedDate).ToList(),
                _ => jobs.OrderBy(j => j.CreatedDate).ToList(),
            };

            var count = jobs.Count;
            var items = jobs.Skip((pageIndex - 1) * pageSize).Take(pageSize);

            return new PaginatedList<JobViewModel>(items, count, pageIndex, pageSize);
        }

        /// <summary>
        /// Retrieves a team by its identifier asynchronously.
        /// </summary>
        /// <param name="id">The team identifier.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation. The task result contains the team view model.</returns>
        public async Task<JobViewModel> GetJobByIdAsync(string id) =>
            _mapper.Map<JobViewModel>(await _repository.FindJobByIdAsync(id));

        public async Task<List<Company>> GetCompaniesWithListingsAsync() =>
            await _repository.GetCompaniesWithListingsAsync();

        public async Task<List<EmploymentType>> GetEmploymentTypesAsync() =>
            await _repository.GetEmploymentTypesAsync();

        public async Task<List<StatusType>> GetStatusTypesAsync() =>
            await _repository.GetStatusTypesAsync();

        public async Task<List<SetupType>> GetWorkSetupsAsync() =>
            await _repository.GetWorkSetupsAsync();

        #endregion Get Methods
    }
}
