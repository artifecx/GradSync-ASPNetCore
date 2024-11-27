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
using Data.Dtos;
using Data.Specifications;
using static Resources.Constants.Enums;

namespace Services.Services
{
    /// <summary>
    /// Service class for handling operations related to teams.
    /// </summary>
    public partial class JobService : IJobService
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

        #region CRUD Methods
        public async Task AddJobAsync(JobViewModel model)
        {
            var job = _mapper.Map<Job>(model);
            var recruiter = await _companyRepository.GetRecruiterByIdAsync(model.PostedById, false);

            if (recruiter == null)
                throw new CompanyException("Recruiter not found.");

            job.JobId = Guid.NewGuid().ToString();
            job.CreatedDate = job.UpdatedDate = DateTime.Now;
            job.StatusTypeId = "Open";
            job.PostedById = recruiter.UserId;
            job.CompanyId = recruiter.CompanyId;
            job.SalaryLower = model.SalaryLower.ToString();
            job.SalaryUpper = model.SalaryUpper.ToString();
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

        public async Task UpdateJobAsync(JobViewModel model)
        {
            var job = await GetJobByIdAsync(model.JobId, true);
            if (job == null)
                throw new JobException("Job not found.");
            if (job.StatusTypeId == "BlackListed")
                throw new JobException("Blacklisted jobs cannot be updated.");

            _mapper.Map(model, job);
            bool jobSkillsChanged = SetJobSkills(job, model);
            bool jobProgramChanged = SetJobPrograms(job, model);
            job.SalaryLower = model.SalaryLower.ToString();
            job.SalaryUpper = model.SalaryUpper.ToString();
            job.Schedule = SetSchedule(model.ScheduleDays, model.ScheduleHours);
            job.SkillWeights = ((decimal)model.SkillWeights);

            if (!_repository.HasChanges(job) && !jobSkillsChanged && !jobProgramChanged)
                throw new JobException("No changes detected.");

            job.UpdatedDate = DateTime.Now;

            await _repository.UpdateJobAsync(job);
            await _jobMatchingApiService.UpdateMatchJobApplicantsAsync(job.JobId);
        }

        public async Task UpdateJobStatusAsync(string jobId, string statusId)
        {
            var job = await GetJobByIdAsync(jobId, true);
            if (job == null)
                throw new JobException("Job not found.");

            var currentStatusId = job.StatusTypeId;
            if (currentStatusId == statusId)
                throw new JobException("No changes detected.");

            if(currentStatusId == "BlackListed")
                throw new JobException("Blacklisted jobs cannot be updated.");

            if (currentStatusId == "Open" && (statusId == "Closed" || statusId == "BlackListed"))
            {
                job.AvailableSlots = 0;
                if (job.Applications.Any())
                {
                    var validApplicationStatuses = new HashSet<string>
                        {
                            "Accepted", "Withdrawn", "Rejected"
                        };

                    foreach (var application in job.Applications)
                    {
                        if (!validApplicationStatuses.Contains(application.ApplicationStatusTypeId))
                        {
                            application.ApplicationStatusTypeId = statusId == "Closed" ? "Rejected" : "Withdrawn";
                            application.UpdatedDate = DateTime.Now;
                        }
                    }
                }
            }
            else
            {
                job.AvailableSlots = job.AvailableSlots == 0 ? 1 : job.AvailableSlots;
            }

            job.StatusTypeId = statusId;
            job.UpdatedDate = DateTime.Now;

            await _repository.UpdateJobAsync(job);
        }

        public async Task ArchiveJobAsync(string id)
        {
            var job = await GetJobByIdAsync(id, true);
            
            if(job.StatusTypeId != "BlackListed" && job.StatusTypeId != "Closed")
            {
                job.StatusTypeId = "Closed";
                job.AvailableSlots = 0;
                var validApplicationStatuses = new HashSet<string>
                {
                    "Accepted", "Withdrawn", "Rejected"
                };

                if (job.Applications.Any() && job.Applications.Count > 0)
                {
                    foreach (var application in job.Applications)
                    {
                        if (!validApplicationStatuses.Contains(application.ApplicationStatusTypeId))
                        {
                            application.ApplicationStatusTypeId = "Rejected";
                            application.UpdatedDate = DateTime.Now;
                        }
                    }
                }
            }

            job.IsArchived = true;
            job.UpdatedDate = DateTime.Now;
            await _repository.UpdateJobAsync(job);
        }

        public async Task UnarchiveJobAsync(JobServiceModel model)
        {
            var job = await GetJobByIdAsync(model.JobId, true, true);

            if (job == null) 
                throw new JobException("Job not found!");

            if (job.StatusTypeId == "BlackListed")
                throw new JobException("Blacklisted jobs cannot be updated.");

            job.IsArchived = false;
            job.StatusTypeId = "Open";
            job.AvailableSlots = model.AvailableSlots.Value;
            job.UpdatedDate = DateTime.Now;
            await _repository.UpdateJobAsync(job);
        }

        public async Task IncrementJobSlots(string jobId)
        {
            var job = await GetJobByIdAsync(jobId, true);
            if (job == null)
                throw new JobException("Job not found!");

            job.AvailableSlots += 1;
            job.UpdatedDate = DateTime.Now;
            await _repository.UpdateJobAsync(job);
        }

        public async Task DecrementJobSlots(string jobId)
        {
            var job = await GetJobByIdAsync(jobId, true);
            if (job == null)
                throw new JobException("Job not found!");

            if (job.AvailableSlots - 1 < 0)
                throw new JobException("No available slots!");
            if (job.AvailableSlots - 1 == 0)
                await CloseJob(job);

            job.AvailableSlots -= 1;
            job.UpdatedDate = DateTime.Now;
            await _repository.UpdateJobAsync(job);
        }

        private static async Task CloseJob(Job job)
        {
            job.StatusTypeId = "Closed";
            var validApplicationStatuses = new HashSet<string>
                {
                    "Accepted", "Withdrawn", "Rejected"
                };

            if (job.Applications.Any() && job.Applications.Count > 0)
            {
                foreach (var application in job.Applications)
                {
                    if (!validApplicationStatuses.Contains(application.ApplicationStatusTypeId))
                    {
                        application.ApplicationStatusTypeId = "Rejected";
                        application.UpdatedDate = DateTime.Now;
                    }
                }
            }

            await Task.CompletedTask;
        }
        #endregion CRUD Methods

        #region Get Methods        
        public async Task<List<FeaturedJobsViewModel>> GetApplicantFeaturedJobsAsync(string userId) =>
            _mapper.Map<List<FeaturedJobsViewModel>>(await _repository.GetApplicantFeaturedJobsAsync(userId));

        public async Task<ApplicantViewDto> GetApplicantDetailsAsync(string applicantId) =>
            await _repository.GetApplicantDetailsAsync(applicantId);

        public async Task<PaginatedList<JobViewModel>> GetAllJobsAsync(FilterServiceModel filters, string archived = null)
        {
            var role = filters.UserRole;
            var userId = role == Role_Recruiter || role == Role_Applicant ? filters.UserId : null;

            var spec = new JobsBaseSpecification();
            spec.AddCriteria(j => string.Equals(archived, "archived") ? j.IsArchived : !j.IsArchived);
            JobIncludes(spec, role, userId);
            FilterAndSortJobs(spec, filters, archived);

            var jobs = await _repository.GetJobsAsync(spec, false);
            var model = _mapper.Map<List<JobViewModel>>(jobs);

            var pageIndex = filters.PageIndex;
            var pageSize = filters.PageSize;
            var count = model.Count;
            var items = model.Skip((pageIndex - 1) * pageSize).Take(pageSize);

            return new PaginatedList<JobViewModel>(items, count, pageIndex, pageSize);
        }

        public async Task<JobViewModel> GetJobByIdAsync(string id)
        {
            var currentUserId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var currentUserRole = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Role).Value;

            var spec = new JobsBaseSpecification();
            spec.AddCriteria(j => j.JobId == id);
            JobIncludes(spec, currentUserRole, currentUserId);

            var job = await _repository.GetJobAsync(spec, false);

            if (job == null)
                throw new JobException("Job not found.");

            var model = _mapper.Map<JobViewModel>(job);

            model.SalaryLower = Convert.ToDouble(job.SalaryLower);
            model.SalaryUpper = Convert.ToDouble(job.SalaryUpper);
            model.ScheduleDays = GetDaysSchedule(job.Schedule);
            model.ScheduleHours = GetHoursSchedule(job.Schedule);

            if (currentUserRole == Role_Applicant)
            {
                var activeApplications = new HashSet<string> { "Submitted", "Viewed", "Shortlisted" };
                var closedApplications = new HashSet<string> { "Accepted", "Rejected" };

                var userApplications = job.Applications
                    .Where(a => a.UserId == currentUserId)
                    .ToHashSet();

                model.HasActiveApplication = userApplications
                    .Any(a => activeApplications.Contains(a.ApplicationStatusTypeId));
                model.HasClosedApplication = userApplications
                    .Any(a => closedApplications.Contains(a.ApplicationStatusTypeId));
                model.HasWithdrawnApplication = userApplications
                    .Any(a => a.ApplicationStatusTypeId == "Withdrawn");
                model.CanReapply = !userApplications
                    .Any(a => a.ApplicationStatusTypeId == "Rejected");

                model.ApplicationStatus = userApplications
                    .OrderByDescending(a => a.CreatedDate)
                    .Select(a => a.ApplicationStatusTypeId)
                    .FirstOrDefault();
                model.ApplicationId = userApplications
                    .OrderByDescending(a => a.CreatedDate)
                    .Select(a => a.ApplicationId)
                    .FirstOrDefault();
            }

            return model;
        }

        public async Task<Job> GetJobByIdAsync(string id, bool track, bool? archived = false)
        {
            var currentUserId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var currentUserRole = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Role).Value;

            var spec = new JobsBaseSpecification();
            spec.AddCriteria(j => j.JobId == id);
            if (archived.HasValue)
                spec.AddCriteria(j => j.IsArchived == archived.Value);

            JobIncludes(spec, currentUserRole, currentUserId);

            var job = await _repository.GetJobAsync(spec, track);

            if (job == null)
                throw new JobException("Job not found.");

            return job;
        }
        #endregion Get Methods

        #region Helper Methods
        private static void JobIncludes(JobsBaseSpecification spec, string currentUserRole, string currentUserId)
        {
            switch (currentUserRole)
            {
                case "Applicant":
                    spec.AddCriteria(j => j.StatusTypeId == "Open");
                    spec.AddInclude(j => j.JobApplicantMatches.Where(jam => jam.UserId == currentUserId));
                    spec.AddInclude(j => j.Applications.Where(a => a.UserId == currentUserId));
                    break;
                default:
                    if (currentUserRole == "Recruiter")
                        spec.AddCriteria(j => j.PostedById == currentUserId);
                    spec.AddInclude(j => j.JobApplicantMatches);
                    spec.AddInclude($"{nameof(Job.JobApplicantMatches)}.{nameof(JobApplicantMatch.User)}");
                    spec.AddInclude($"{nameof(Job.JobApplicantMatches)}.{nameof(JobApplicantMatch.User)}.{nameof(Applicant.User)}");
                    spec.AddInclude(j => j.Applications);
                    break;
            }

            spec.AddInclude(j => j.JobSkills);
            spec.AddInclude($"{nameof(Job.JobSkills)}.{nameof(JobSkill.Skill)}");
            spec.AddInclude(j => j.JobPrograms);
            spec.AddInclude($"{nameof(Job.JobPrograms)}.{nameof(JobProgram.Program)}");
            spec.AddInclude(j => j.YearLevel);
            spec.AddInclude(j => j.EmploymentType);
            spec.AddInclude(j => j.SetupType);
            spec.AddInclude(j => j.StatusType);
            spec.AddInclude(j => j.PostedBy);
            spec.AddInclude($"{nameof(Job.PostedBy)}.{nameof(Recruiter.User)}");
            spec.AddInclude(j => j.Company);
        }

        private void FilterAndSortJobs(JobsBaseSpecification spec, FilterServiceModel filters, string archived = null)
        {
            var search = filters.Search;
            var filterByDatePosted = filters.FilterByDatePosted;
            var filterByCompany = filters.FilterByCompany;
            var filterByEmploymentType = filters.FilterByEmploymentType;
            var filterByStatusType = filters.FilterByStatusType;
            var filterByWorkSetup = filters.FilterByWorkSetup;
            var filterBySalary = filters.FilterBySalary;
            var sortBy = filters.SortBy;
            var userRole = filters.UserRole;
            var userId = filters.UserId;

            if (!string.IsNullOrEmpty(userRole) && userRole == Role_Applicant)
            {
                var applicant = _repository.GetApplicantDetailsAsync(userId).Result;
                if (applicant != null)
                {
                    spec.AddCriteria(j => j.JobPrograms.Any(jp => jp.Program.DepartmentId == applicant.DepartmentId));
                }
            }

            if (!string.IsNullOrEmpty(search))
            {
                if (string.IsNullOrEmpty(archived))
                {

                    spec.AddCriteria(j =>
                        j.Title.Contains(search) ||
                        j.Description.Contains(search) ||
                        j.Company.Name.Contains(search) ||
                        j.EmploymentTypeId.Contains(search) ||
                        j.SetupTypeId.Contains(search) ||
                        j.StatusTypeId.Contains(search)
                    );
                }
                else
                {
                    spec.AddCriteria(
                        j => j.Title.Contains(search) ||
                        j.Description.Contains(search)
                    );
                }
            }

            if (!string.IsNullOrEmpty(filterByDatePosted))
            {
                DateTime today = DateTime.Today;
                DateTime tomorrow = today.AddDays(1);

                int diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
                DateTime weekStart = today.AddDays(-1 * diff).Date;
                DateTime weekEnd = weekStart.AddDays(7);

                DateTime monthStart = new(today.Year, today.Month, 1);
                DateTime monthEnd = monthStart.AddMonths(1);

                spec.AddCriteria(filterByDatePosted switch
                {
                    "today" => j => j.CreatedDate >= today && j.CreatedDate < tomorrow,
                    "week" => j => j.CreatedDate >= weekStart && j.CreatedDate < weekEnd,
                    "month" => j => j.CreatedDate >= monthStart && j.CreatedDate < monthEnd,
                    _ => null
                });
            }

            if (!string.IsNullOrEmpty(filterByCompany))
            {
                spec.AddCriteria(j => j.Company.Name == filterByCompany);
            }

            if (filterByEmploymentType.Any())
            {
                spec.AddCriteria(j => filterByEmploymentType.Contains(j.EmploymentType.Name));
            }

            if (!string.IsNullOrEmpty(filterByStatusType))
            {
                spec.AddCriteria(j => j.StatusType.Name == filterByStatusType);
            }

            if (filterByWorkSetup.Any())
            {
                spec.AddCriteria(j => filterByWorkSetup.Contains(j.SetupType.Name));
            }

            if (int.TryParse(filterBySalary, out int salary))
            {
                spec.AddCriteria(j => Convert.ToInt32(j.SalaryLower) <= salary);
            }

            switch (sortBy)
            {
                case "created_desc":
                    spec.ApplyOrderByDescending(j => j.CreatedDate);
                    break;
                case "created_asc":
                    spec.ApplyOrderBy(j => j.CreatedDate);
                    break;
                case "salary_desc":
                    spec.ApplyOrderByDescending(j => Convert.ToInt32(j.SalaryUpper));
                    spec.ApplyThenOrderByDescending(j => Convert.ToInt32(j.SalaryLower));
                    break;
                case "salary_asc":
                    spec.ApplyOrderBy(j => Convert.ToInt32(j.SalaryLower));
                    spec.ApplyThenOrderBy(j => Convert.ToInt32(j.SalaryUpper));
                    break;
                case "updated_desc":
                    spec.ApplyOrderByDescending(j => j.UpdatedDate);
                    break;
                case "updated_asc":
                    spec.ApplyOrderBy(j => j.UpdatedDate);
                    break;
                case "match":
                    spec.ApplyOrderByDescending(j => j.JobApplicantMatches
                        .Where(m => m.UserId == userId)
                        .Max(m => m.MatchPercentage));
                    break;
                default:
                    if (userRole == Role_Applicant)
                        spec.ApplyOrderByDescending(j => j.JobApplicantMatches
                            .Where(m => m.UserId == userId)
                            .Max(m => m.MatchPercentage));
                    else spec.ApplyOrderByDescending(j => j.CreatedDate);
                    break;
            }
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

        private static string SetSchedule(int? days, int? hours)
        {
            if (days.HasValue && hours.HasValue)
            {
                var daysValue = days.Value == 0 ? "Flexible" : days.Value.ToString();
                var hoursValue = hours.Value == 0 ? "Flexible" : hours.Value.ToString();
                var daysString = days.Value == 1 ? "day" : "days";
                var hoursString = hours.Value == 1 ? "hour" : "hours";

                return $"{daysValue} {daysString}, {hoursValue} {hoursString}";
            }
            throw new InvalidOperationException("Invalid schedule!");
        }

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
        #endregion Helper Methods
    }
}
