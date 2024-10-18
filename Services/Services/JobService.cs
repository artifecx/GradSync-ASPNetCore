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
using System.Globalization;

namespace Services.Services
{
    /// <summary>
    /// Service class for handling operations related to teams.
    /// </summary>
    public class JobService : IJobService
    {
        private readonly IJobRepository _repository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public JobService(
            IJobRepository repository,
            IMapper mapper,
            ILogger<JobService> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _repository = repository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
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
            List<string> filterByEmploymentType, string filterByStatusType,
            List<string> filterByWorkSetup, int pageIndex, int pageSize, 
            string filterByDatePosted = null, string filterBySalary = null)
        {
            var jobs = _mapper.Map<List<JobViewModel>>(await _repository.GetAllJobsAsync());
            jobs = await FilterAndSortJobs(jobs, sortBy, search, filterByCompany, 
                filterByEmploymentType, filterByStatusType, filterByWorkSetup,
                filterByDatePosted, filterBySalary);

            var count = jobs.Count;
            var items = jobs.Skip((pageIndex - 1) * pageSize).Take(pageSize);

            return new PaginatedList<JobViewModel>(items, count, pageIndex, pageSize);
        }

        public async Task<List<JobViewModel>> GetAllJobsAsync() =>
            _mapper.Map <List<JobViewModel>>(await _repository.GetAllJobsAsync());

        public async Task<PaginatedList<JobViewModel>> GetRecruiterJobsAsync(
            string sortBy, string search, string filterByCompany,
            List<string> filterByEmploymentType, string filterByStatusType,
            List<string> filterByWorkSetup, int pageIndex, int pageSize)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var jobs = _mapper.Map<List<JobViewModel>>(await _repository.GetRecruiterJobsAsync(userId));
            jobs = await FilterAndSortJobs(jobs, sortBy, search, filterByCompany, 
                filterByEmploymentType, filterByStatusType, filterByWorkSetup);

            var count = jobs.Count;
            var items = jobs.Skip((pageIndex - 1) * pageSize).Take(pageSize);

            return new PaginatedList<JobViewModel>(items, count, pageIndex, pageSize);
        }

        private async Task<List<JobViewModel>> FilterAndSortJobs(
            List<JobViewModel> jobs, string sortBy, string search, 
            string filterByCompany, List<string> filterByEmploymentType, 
            string filterByStatusType, List<string> filterByWorkSetup, 
            string filterByDatePosted = null, string filterBySalary = null)
        {
            if (!string.IsNullOrEmpty(search))
            {
                jobs = jobs
                    .Where(job => 
                        job.Title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                        job.Description.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                        job.Company.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                        job.EmploymentTypeId.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                        job.SetupTypeId.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                        job.StatusTypeId.Contains(search, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (!string.IsNullOrEmpty(filterByDatePosted))
            {
                DateTime today = DateTime.Today;
                DateTime tomorrow = today.AddDays(1);

                int diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
                DateTime weekStart = today.AddDays(-1 * diff).Date;
                DateTime weekEnd = weekStart.AddDays(7);

                DateTime monthStart = new DateTime(today.Year, today.Month, 1);
                DateTime monthEnd = monthStart.AddMonths(1);

                jobs = filterByDatePosted switch
                {
                    "today" => jobs.Where(job => job.CreatedDate >= today && job.CreatedDate < tomorrow).ToList(),
                    "week" => jobs.Where(job => job.CreatedDate >= weekStart && job.CreatedDate < weekEnd).ToList(),
                    "month" => jobs.Where(job => job.CreatedDate >= monthStart && job.CreatedDate < monthEnd).ToList(),
                    _ => jobs
                };
            }

            if (!string.IsNullOrEmpty(filterByCompany))
            {
                jobs = jobs.Where(job => job.Company.Name == filterByCompany).ToList();
            }

            if (filterByEmploymentType.Any())
            {
                jobs = jobs.Where(job => filterByEmploymentType.Contains(job.EmploymentType.Name)).ToList();
            }

            if (!string.IsNullOrEmpty(filterByStatusType))
            {
                jobs = jobs.Where(job => job.StatusType.Name == filterByStatusType).ToList();
            }

            if (filterByWorkSetup.Any())
            {
                jobs = jobs.Where(job => filterByWorkSetup.Contains(job.SetupType.Name)).ToList();
            }

            jobs = sortBy switch
            {
                "created_desc" => jobs.OrderByDescending(j => j.CreatedDate).ToList(),
                "created_asc" => jobs.OrderBy(j => j.CreatedDate).ToList(),
                "salary_desc" => jobs.OrderByDescending(j => GetLowerSalary(j.Salary)).ThenByDescending(j => GetUpperSalary(j.Salary)).ToList(),
                "salary_asc" => jobs.OrderBy(j => GetLowerSalary(j.Salary)).ThenBy(j => GetUpperSalary(j.Salary)).ToList(),
                //"match" => jobs.OrderByDescending(j => j.CreatedDate).ToList(),
                _ => jobs.OrderByDescending(j => j.CreatedDate).ToList(),
            };

            return jobs;
        }

        /// <summary>
        /// Retrieves a team by its identifier asynchronously.
        /// </summary>
        /// <param name="id">The team identifier.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation. The task result contains the team view model.</returns>
        public async Task<JobViewModel> GetJobByIdAsync(string id) =>
            _mapper.Map<JobViewModel>(await _repository.GetJobByIdAsync(id));

        public async Task<List<Company>> GetCompaniesWithListingsAsync() =>
            await _repository.GetCompaniesWithListingsAsync();

        public async Task<List<EmploymentType>> GetEmploymentTypesAsync() =>
            await _repository.GetEmploymentTypesAsync();

        public async Task<List<StatusType>> GetStatusTypesAsync() =>
            await _repository.GetStatusTypesAsync();

        public async Task<List<SetupType>> GetWorkSetupsAsync() =>
            await _repository.GetWorkSetupsAsync();

        public async Task<List<Department>> GetDepartmentsAsync() =>
            await _repository.GetDepartmentsAsync();

        public async Task<List<Skill>> GetSkillsAsync() =>
            await _repository.GetSkillsAsync();

        public async Task<List<YearLevel>> GetYearLevelsAsync() =>
            await _repository.GetYearLevelsAsync();

        #endregion Get Methods

        /// <summary>
        /// Extracts the lower salary from the salary range string.
        /// </summary>
        private static int GetLowerSalary(string salaryRange)
        {
            if (string.IsNullOrWhiteSpace(salaryRange))
                return 0;

            var parts = salaryRange.Split('-');
            if (parts.Length == 0)
                return 0;

            var lowerPart = parts[0].Trim();

            var lowerSalaryStr = lowerPart.Replace("Php", "").Replace(",", "").Trim();

            if (int.TryParse(lowerSalaryStr, out int lowerSalary))
                return lowerSalary;

            return 0;
        }

        /// <summary>
        /// Extracts the upper salary from the salary range string.
        /// </summary>
        private static int GetUpperSalary(string salaryRange)
        {
            if (string.IsNullOrWhiteSpace(salaryRange))
                return 0;

            var parts = salaryRange.Split('-');
            if (parts.Length < 2)
                return 0;

            var upperPart = parts[1].Trim();

            var upperSalaryStr = upperPart.Replace("Php", "").Replace(",", "").Trim();

            if (int.TryParse(upperSalaryStr, out int upperSalary))
                return upperSalary;

            return 0;
        }
    }
}
