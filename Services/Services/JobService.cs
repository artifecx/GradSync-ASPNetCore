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
using static Services.Exceptions.CompanyExceptions;
using static Resources.Constants.UserRoles;
using Data.Repositories;

namespace Services.Services
{
    /// <summary>
    /// Service class for handling operations related to teams.
    /// </summary>
    public class JobService : IJobService
    {
        private readonly IJobRepository _repository;
        private readonly ICompanyRepository _companyRepository;
        private readonly IJobMatchingApiService _jobMatchingApiService;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public JobService(
            IJobRepository repository,
            ICompanyRepository companyRepository,
            IJobMatchingApiService jobMatchingApiService,
            IMapper mapper,
            ILogger<JobService> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _repository = repository;
            _companyRepository = companyRepository;
            _jobMatchingApiService = jobMatchingApiService;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task AddJobAsync(JobViewModel model)
        {
            var job = _mapper.Map<Job>(model);
            var recruiter = await _companyRepository.GetRecruiterByIdAsync(model.PostedById, false);

            if (recruiter == null)
                throw new CompanyException("Recruiter not found.");

            job.JobId = Guid.NewGuid().ToString();
            job.CreatedDate = job.UpdatedDate = DateTime.Now;
            job.StatusTypeId = "Pending";
            job.PostedById = recruiter.UserId;
            job.CompanyId = recruiter.CompanyId;
            job.Salary = SetSalaryRange(model.SalaryLower, model.SalaryUpper);
            job.Schedule = SetSchedule(model.ScheduleDays, model.ScheduleHours);
            job.JobSkills = new[]
            {
                new { Skills = model.SkillsT ?? Enumerable.Empty<Skill>(), Type = "Technical" },
                new { Skills = model.SkillsS ?? Enumerable.Empty<Skill>(), Type = "Cultural" },
                new { Skills = model.SkillsC ?? Enumerable.Empty<Skill>(), Type = "Certification" }
            }
            .SelectMany(group => group.Skills.Select(skill => new JobSkill
            {
                JobSkillId = Guid.NewGuid().ToString(),
                JobId = job.JobId,
                SkillId = skill.SkillId,
                Type = group.Type
            }))
            .ToList();
            job.JobPrograms = model.Programs.Select(program => new JobProgram
            {
                JobProgramId = Guid.NewGuid().ToString(),
                JobId = job.JobId,
                ProgramId = program.ProgramId
            }).ToList();
            job.SkillWeights = ((decimal)model.SkillWeights);

            await _repository.AddJobAsync(job);
            await _jobMatchingApiService.MatchAndSaveJobApplicantsAsync(job.JobId);
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
            bool jobProgramChanged = SetJobPrograms(job, model);
            job.Salary = SetSalaryRange(model.SalaryLower, model.SalaryUpper);
            job.Schedule = SetSchedule(model.ScheduleDays, model.ScheduleHours);
            job.SkillWeights = ((decimal)model.SkillWeights);

            if (!_repository.HasChanges(job) && !jobSkillsChanged && !jobProgramChanged)
                throw new CompanyException("No changes detected.");

            job.UpdatedDate = DateTime.Now;

            await _repository.UpdateJobAsync(job);
        }

        private bool SetJobSkills(Job job, JobViewModel model)
        {
            var jobSkillC = model.SkillsC != null && model.SkillsC.Any() 
                ? CreateJobSkills(model.SkillsC, "Certification", job.JobId) 
                : new List<JobSkill>().AsEnumerable();
            var newJobSkills = CreateJobSkills(model.SkillsS, "Cultural", job.JobId)
                .Concat(CreateJobSkills(model.SkillsT, "Technical", job.JobId))
                .Concat(jobSkillC)
                .ToList();

            var currentJobSkills = job.JobSkills.ToList();

            var newJobSkillKeys = new HashSet<(string SkillId, string Type)>(
                newJobSkills.Select(js => (js.SkillId, js.Type))
            );
            var currentJobSkillKeys = new HashSet<(string SkillId, string Type)>(
                currentJobSkills.Select(js => (js.SkillId, js.Type))
            );

            var skillsToRemove = currentJobSkills
                .Where(js => !newJobSkillKeys.Contains((js.SkillId, js.Type)))
                .ToList();

            var skillsToAdd = newJobSkills
                .Where(js => !currentJobSkillKeys.Contains((js.SkillId, js.Type)))
                .ToList();

            foreach (var jobSkill in skillsToRemove)
            {
                job.JobSkills.Remove(jobSkill);
            }

            foreach (var jobSkill in skillsToAdd)
            {
                job.JobSkills.Add(new JobSkill
                {
                    JobSkillId = Guid.NewGuid().ToString(),
                    JobId = job.JobId,
                    SkillId = jobSkill.SkillId,
                    Type = jobSkill.Type
                });
            }

            return skillsToAdd.Any() || skillsToRemove.Any();
        }

        /// <summary>
        /// Helper method to create JobSkill instances from a list of skills and a specified type.
        /// </summary>
        /// <param name="skills">The collection of skills.</param>
        /// <param name="type">The type of the skills (e.g., Technical, Certification, Cultural).</param>
        /// <param name="jobId">The ID of the job.</param>
        /// <returns>An enumerable of JobSkill instances.</returns>
        private IEnumerable<JobSkill> CreateJobSkills(IEnumerable<Skill> skills, string type, string jobId)
        {
            return skills.Select(skill => new JobSkill
            {
                JobSkillId = Guid.NewGuid().ToString(),
                JobId = jobId,
                SkillId = skill.SkillId,
                Type = type
            });
        }

        private bool SetJobPrograms(Job job, JobViewModel model)
        {
            var currentJobPrograms = job.JobPrograms.Select(js => js.ProgramId).ToList();
            var newPrograms = model.Programs.Select(s => s.ProgramId).ToList();

            var programsToRemove = currentJobPrograms.Except(newPrograms).ToList();
            if (programsToRemove.Any())
            {
                foreach (var programId in programsToRemove)
                {
                    var jobProgram = job.JobPrograms.FirstOrDefault(js => js.ProgramId == programId);
                    if (jobProgram != null)
                    {
                        job.JobPrograms.Remove(jobProgram);
                    }
                }
            }
            var programsToAdd = newPrograms.Except(currentJobPrograms).ToList();
            if (programsToAdd.Any())
            {
                foreach (var programId in programsToAdd)
                {
                    var jobProgram = new JobProgram
                    {
                        JobProgramId = Guid.NewGuid().ToString(),
                        JobId = job.JobId,
                        ProgramId = programId
                    };
                    job.JobPrograms.Add(jobProgram);
                }
            }
            return programsToAdd.Any() || programsToRemove.Any();
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
        public async Task<List<FeaturedJobsViewModel>> GetApplicantFeaturedJobsAsync(string userId)
        {
            var jobs = await _repository.GetApplicantFeaturedJobsAsync(userId);
            return _mapper.Map<List<FeaturedJobsViewModel>>(jobs);
        }

        public async Task<PaginatedList<JobViewModel>> GetAllJobsAsync(FilterServiceModel filters, string archived = null)
        {
            var jobs = string.Equals(archived, "archived") ?
                _mapper.Map<List<JobViewModel>>(await _repository.GetArchivedJobsAsync()) :
                _mapper.Map<List<JobViewModel>>(await _repository.GetAllJobsAsync());
            jobs = await FilterAndSortJobs(jobs, filters, archived);

            var pageIndex = filters.PageIndex;
            var pageSize = filters.PageSize;
            var count = jobs.Count;
            var items = jobs.Skip((pageIndex - 1) * pageSize).Take(pageSize);

            return new PaginatedList<JobViewModel>(items, count, pageIndex, pageSize);
        }

        public async Task<List<JobViewModel>> GetAllJobsAsync() =>
            _mapper.Map <List<JobViewModel>>(await _repository.GetAllJobsAsync());

        public async Task<PaginatedList<JobViewModel>> GetRecruiterJobsAsync(FilterServiceModel filters, string archived = null)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var jobs = string.Equals(archived, "archived") ? 
                _mapper.Map<List<JobViewModel>>(await _repository.GetRecruiterArchivedJobsAsync(userId)) :
                _mapper.Map<List<JobViewModel>>(await _repository.GetRecruiterJobsAsync(userId));
            jobs = await FilterAndSortJobs(jobs, filters, archived);

            var pageIndex = filters.PageIndex;
            var pageSize = filters.PageSize;
            var count = jobs.Count;
            var items = jobs.Skip((pageIndex - 1) * pageSize).Take(pageSize);

            return new PaginatedList<JobViewModel>(items, count, pageIndex, pageSize);
        }

        private async Task<List<JobViewModel>> FilterAndSortJobs(List<JobViewModel> jobs, FilterServiceModel filters, string archived = null)
        {
            var search = filters.Search;
            var filterByDatePosted = filters.FilterByDatePosted;
            var filterByCompany = filters.FilterByCompany;
            var filterByEmploymentType = filters.FilterByEmploymentType;
            var filterByStatusType = filters.FilterByStatusType;
            var filterByWorkSetup = filters.FilterByWorkSetup;
            var filterBySalary = filters.FilterBySalary;
            var sortBy = filters.SortBy;

            if (!string.IsNullOrEmpty(search))
            {
                if (string.IsNullOrEmpty(archived))
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
            var currentUserId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var currentUserRole = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Role).Value;

            if (job == null)
                throw new JobException("Job not found.");

            var model = _mapper.Map<JobViewModel>(job);

            if (currentUserRole == Role_Applicant)
            {
                model.HasApplied = job.Applications.Any(a => a.UserId == currentUserId);
                model.ApplicationId = job.Applications.FirstOrDefault(a => a.UserId == currentUserId)?.ApplicationId;
            }
                
            model.SalaryLower = GetLowerSalary(job.Salary);
            model.SalaryUpper = GetUpperSalary(job.Salary);
            model.ScheduleDays = GetDaysSchedule(job.Schedule);
            model.ScheduleHours = GetHoursSchedule(job.Schedule);

            return model;
        }
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
