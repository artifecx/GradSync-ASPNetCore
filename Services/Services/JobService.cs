using Data.Interfaces;
using Data.Models;
using static Services.Exceptions.JobExceptions;
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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static Services.Exceptions.CompanyExceptions;

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
            var job = _mapper.Map<Job>(model);
            job.JobId = Guid.NewGuid().ToString();
            job.CreatedDate = DateTime.Now;
            job.UpdatedDate = DateTime.Now;
            job.StatusTypeId = "Open";
            job.PostedById = model.PostedById;
            job.Salary = SetSalaryRange(model.SalaryLower, model.SalaryUpper);
            job.Schedule = SetSchedule(model.ScheduleDays, model.ScheduleHours);
            job.JobSkills = model.Skills.Select(skill => new JobSkill
            {
                JobSkillId = Guid.NewGuid().ToString(),
                JobId = job.JobId,
                SkillId = skill.SkillId
            }).ToList();

            job.JobDepartments = model.Departments.Select(department => new JobDepartment
            {
                JobDepartmentId = Guid.NewGuid().ToString(),
                JobId = job.JobId,
                DepartmentId = department.DepartmentId
            }).ToList();

            await _repository.AddJobAsync(job);
        }

        private static string SetSalaryRange(double? lower, double? upper)
        {
            var salary = string.Empty;
            if(lower.HasValue && upper.HasValue)
            {
                var lowerValue = lower.Value;
                var upperValue = upper.Value;

                if (lowerValue > upperValue)
                    salary = $"Php {upperValue.ToString("N0")} - Php {lowerValue.ToString("N0")}";
                else
                    salary = $"Php {lowerValue.ToString("N0")} - Php {upperValue.ToString("N0")}";

                return salary;
            }
            throw new InvalidOperationException("Invalid salary range!");
        }

        private static string SetSchedule(int? days, int? hours)
        {
            if(days.HasValue && hours.HasValue)
            {
                var daysValue = days.Value == 0 ? "Flexible" : days.Value.ToString();
                var hoursValue = hours.Value == 0 ? "Flexible" : hours.Value.ToString();
                var daysString = days.Value == 1 ? "day" : "days";
                var hoursString = hours.Value == 1 ? "hour" : "hours";

                return $"{daysValue} {daysString}, {hoursValue} {hoursString}";
            }
            throw new InvalidOperationException("Invalid schedule!");
        }

        public async Task UpdateJobAsync(JobViewModel model)
        {
            var job = await _repository.GetJobByIdAsync(model.JobId, true);
            if (job == null)
                throw new JobException("Job not found.");

            _mapper.Map(model, job);
            bool jobSkillsChanged = SetJobSkills(job, model);
            bool jobDepartmentChanged = SetJobDepartments(job, model);
            job.Salary = SetSalaryRange(model.SalaryLower, model.SalaryUpper);
            job.Schedule = SetSchedule(model.ScheduleDays, model.ScheduleHours);

            if (!_repository.HasChanges(job) && !jobSkillsChanged && !jobDepartmentChanged)
                throw new CompanyException("No changes detected.");

            job.UpdatedDate = DateTime.Now;

            await _repository.UpdateJobAsync(job);
        }

        private bool SetJobSkills(Job job, JobViewModel model)
        {
            var currentJobSkills = job.JobSkills.Select(js => js.SkillId).ToList();
            var newSkills = model.Skills.Select(s => s.SkillId).ToList();

            var skillsToRemove = currentJobSkills.Except(newSkills).ToList();
            if (skillsToRemove.Any())
            {
                foreach (var skillId in skillsToRemove)
                {
                    var jobSkill = job.JobSkills.FirstOrDefault(js => js.SkillId == skillId);
                    if (jobSkill != null)
                    {
                        job.JobSkills.Remove(jobSkill);
                    }
                }
            }
            var skillsToAdd = newSkills.Except(currentJobSkills).ToList();
            if (skillsToAdd.Any())
            {
                foreach (var skillId in skillsToAdd)
                {
                    var jobSkill = new JobSkill
                    {
                        JobSkillId = Guid.NewGuid().ToString(),
                        JobId = job.JobId,
                        SkillId = skillId
                    };
                    job.JobSkills.Add(jobSkill);
                }
            }
            return skillsToAdd.Any() || skillsToRemove.Any();
        }

        private bool SetJobDepartments(Job job, JobViewModel model)
        {
            var currentJobDepartments = job.JobDepartments.Select(js => js.DepartmentId).ToList();
            var newDepartments = model.Departments.Select(s => s.DepartmentId).ToList();

            var departmentsToRemove = currentJobDepartments.Except(newDepartments).ToList();
            if (departmentsToRemove.Any())
            {
                foreach (var departmentId in departmentsToRemove)
                {
                    var jobDepartment = job.JobDepartments.FirstOrDefault(js => js.DepartmentId == departmentId);
                    if (jobDepartment != null)
                    {
                        job.JobDepartments.Remove(jobDepartment);
                    }
                }
            }
            var departmentsToAdd = newDepartments.Except(currentJobDepartments).ToList();
            if (departmentsToAdd.Any())
            {
                foreach (var departmentId in departmentsToAdd)
                {
                    var jobDepartment = new JobDepartment
                    {
                        JobDepartmentId = Guid.NewGuid().ToString(),
                        JobId = job.JobId,
                        DepartmentId = departmentId
                    };
                    job.JobDepartments.Add(jobDepartment);
                }
            }
            return departmentsToAdd.Any() || departmentsToRemove.Any();
        }

        public async Task ArchiveJobAsync(string id)
        {
            var job = await _repository.GetJobByIdAsync(id, true);
            job.IsArchived = true;
            job.StatusTypeId = "Closed";
            job.AvailableSlots = 0;
            if(job.Applications.Any())
            {
                foreach (var application in job.Applications)
                {
                    application.ApplicationStatusTypeId = "Rejected";
                    application.UpdatedDate = DateTime.Now;
                }
            }
            job.UpdatedDate = DateTime.Now;
            await _repository.UpdateJobAsync(job);
        }

        public async Task UnarchiveJobAsync(JobServiceModel model)
        {
            var job = await _repository.GetJobByIdAsync(model.JobId, "true");

            if (job == null) throw new JobException("Job not found!");

            job.IsArchived = false;
            job.StatusTypeId = "Open";
            job.AvailableSlots = model.AvailableSlots.Value;
            job.UpdatedDate = DateTime.Now;
            await _repository.UpdateJobAsync(job);
        }

        #region Get Methods        
        public async Task<PaginatedList<JobViewModel>> GetAllJobsAsync(
            string sortBy, string search, string filterByCompany,
            List<string> filterByEmploymentType, string filterByStatusType,
            List<string> filterByWorkSetup, int pageIndex, int pageSize, 
            string filterByDatePosted = null, string filterBySalary = null, string archived = null)
        {
            var jobs = string.Equals(archived, "archived") ?
                _mapper.Map<List<JobViewModel>>(await _repository.GetArchivedJobsAsync()) :
                _mapper.Map<List<JobViewModel>>(await _repository.GetAllJobsAsync());
            jobs = await FilterAndSortJobs(jobs, sortBy, search, filterByCompany, 
                filterByEmploymentType, filterByStatusType, filterByWorkSetup,
                filterByDatePosted, filterBySalary, archived);

            var count = jobs.Count;
            var items = jobs.Skip((pageIndex - 1) * pageSize).Take(pageSize);

            return new PaginatedList<JobViewModel>(items, count, pageIndex, pageSize);
        }

        public async Task<List<JobViewModel>> GetAllJobsAsync() =>
            _mapper.Map <List<JobViewModel>>(await _repository.GetAllJobsAsync());

        public async Task<PaginatedList<JobViewModel>> GetRecruiterJobsAsync(
            string sortBy, string search, string filterByCompany,
            List<string> filterByEmploymentType, string filterByStatusType,
            List<string> filterByWorkSetup, int pageIndex, int pageSize, string archived = null)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var jobs = string.Equals(archived, "archived") ? 
                _mapper.Map<List<JobViewModel>>(await _repository.GetRecruiterArchivedJobsAsync(userId)) :
                _mapper.Map<List<JobViewModel>>(await _repository.GetRecruiterJobsAsync(userId));
            jobs = await FilterAndSortJobs(jobs, sortBy, search, filterByCompany, 
                filterByEmploymentType, filterByStatusType, filterByWorkSetup, archived);

            var count = jobs.Count;
            var items = jobs.Skip((pageIndex - 1) * pageSize).Take(pageSize);

            return new PaginatedList<JobViewModel>(items, count, pageIndex, pageSize);
        }

        private async Task<List<JobViewModel>> FilterAndSortJobs(
            List<JobViewModel> jobs, string sortBy, string search, 
            string filterByCompany, List<string> filterByEmploymentType, 
            string filterByStatusType, List<string> filterByWorkSetup, 
            string filterByDatePosted = null, string filterBySalary = null,
            string archived = null)
        {
            if (!string.IsNullOrEmpty(search))
            {
                if (!string.IsNullOrEmpty(archived))
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
                else
                {
                    jobs = jobs
                        .Where(job =>
                            job.Title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                            job.Description.Contains(search, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }
            }

            if (!string.IsNullOrEmpty(filterByDatePosted))
            {
                DateTime today = DateTime.Today;
                DateTime tomorrow = today.AddDays(1);

                int diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
                DateTime weekStart = today.AddDays(-1 * diff).Date;
                DateTime weekEnd = weekStart.AddDays(7);

                DateTime monthStart = new (today.Year, today.Month, 1);
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

            if (int.TryParse(filterBySalary, out int salary))
            {
                jobs = jobs.Where(j => GetLowerSalary(j.Salary) <= salary).ToList();
            }

            jobs = sortBy switch
            {
                "created_desc" => jobs.OrderByDescending(j => j.CreatedDate).ToList(),
                "created_asc" => jobs.OrderBy(j => j.CreatedDate).ToList(),
                "salary_desc" => jobs.OrderByDescending(j => GetUpperSalary(j.Salary)).ThenByDescending(j => GetLowerSalary(j.Salary)).ToList(),
                "salary_asc" => jobs.OrderBy(j => GetLowerSalary(j.Salary)).ThenBy(j => GetUpperSalary(j.Salary)).ToList(),
                "updated_desc" => jobs.OrderByDescending(j => j.UpdatedDate).ToList(),
                "updated_asc" => jobs.OrderBy(j => j.UpdatedDate).ToList(),
                //"match" => jobs.OrderByDescending(j => j.CreatedDate).ToList(),
                _ => jobs.OrderByDescending(j => j.CreatedDate).ToList(),
            };

            return jobs;
        }

        /// <summary>
        /// Retrieves a job by its identifier asynchronously.
        /// </summary>
        /// <param name="id">The job identifier.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation. 
        /// The task result contains the team view model.</returns>
        public async Task<JobViewModel> GetJobByIdAsync(string id) 
        {
            var job = await _repository.GetJobByIdAsync(id, false);
            var model = _mapper.Map<JobViewModel>(job);

            model.SalaryLower = GetLowerSalary(job.Salary);
            model.SalaryUpper = GetUpperSalary(job.Salary);
            model.ScheduleDays = GetDaysSchedule(job.Schedule);
            model.ScheduleHours = GetHoursSchedule(job.Schedule);

            return model;
        }
                       

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

        /// <summary>
        /// Extracts the days from the schedule string.
        /// </summary>
        private static int GetDaysSchedule(string schedule)
        {
            if (string.IsNullOrWhiteSpace(schedule))
                return 0;

            var parts = schedule.Split(',');
            if (parts.Length < 2)
                return 0;

            var daysPart = parts[0].Trim();

            var daysString = daysPart.Replace("days", "").Replace("day", "").Replace(",", "").Trim();

            if (string.Equals(daysString, "Flexible"))
                return 0;

            if (int.TryParse(daysString, out int days))
                return days;

            return 0;
        }

        /// <summary>
        /// Extracts the hours from the schedule string.
        /// </summary>
        private static int GetHoursSchedule(string schedule)
        {
            if (string.IsNullOrWhiteSpace(schedule))
                return 0;

            var parts = schedule.Split(',');
            if (parts.Length < 2)
                return 0;

            var hoursPart = parts[1].Trim();

            var hoursString = hoursPart.Replace("hours", "").Replace("hour", "").Replace(",", "").Trim();

            if (string.Equals(hoursString, "Flexible"))
                return 0;

            if (int.TryParse(hoursString, out int hours))
                return hours;

            return 0;
        }
    }
}
