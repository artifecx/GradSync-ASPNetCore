using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Data.Interfaces;
using Data.Models;
using Services.Interfaces;
using Services.EventBus;
using Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Resources.Constants.CacheKeys;
using static Resources.Constants.ExpirationTimes;
using static Resources.Constants.UserRoles;
using static Resources.Constants.Types;
using static Resources.Messages.ErrorMessages;
using static Services.Exceptions.JobApplicationExceptions;
using Microsoft.Extensions.Logging;

namespace Services.Services
{
    /// <summary>
    /// Service class for handling operations related to job applications.
    /// </summary>
    public class ApplicationService : IApplicationService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IEventBus _eventBus;
        private readonly IMapper _mapper;
        private static TimeSpan cacheExpirationMinutes = TimeSpan.FromMinutes(Convert.ToInt32(Expiration_Applications));

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationService"/> class.
        /// </summary>
        /// <param name="serviceProvider">Service provider for dependency injection.</param>
        /// <param name="memoryCache">In-memory cache for storing application data.</param>
        /// <param name="eventBus">Event bus for publishing events related to application data.</param>
        /// <param name="mapper">Object mapper for converting between data and service models.</param>
        public ApplicationService
            (IServiceProvider serviceProvider, 
            IEventBus eventBus, 
            IMapper mapper,
            ILogger<ApplicationService> logger)
        {
            _serviceProvider = serviceProvider;
            _eventBus = eventBus;
            _mapper = mapper;
        }

        /// <summary>
        /// Sends a job application for a specific user and job.
        /// </summary>
        /// <param name="userId">The ID of the user submitting the application.</param>
        /// <param name="jobId">The ID of the job for which the application is submitted.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task SendApplicationAsync(string userId, string jobId)
        {
            if (await HasExistingApplicationAsync(userId, jobId))
                throw new JobApplicationException(Error_ApplicationExists);

            var application = new Application
            {
                ApplicationId = Guid.NewGuid().ToString(),
                ApplicationStatusTypeId = AppStatus_Submitted,
                UserId = userId,
                JobId = jobId,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now
            };

            using (var scope = _serviceProvider.CreateScope())
            {
                var repository = scope.ServiceProvider.GetRequiredService<IApplicationRepository>();
                await repository.AddApplicationAsync(application);
            }

            await UpdateApplicationCacheAsync(userId, application.ApplicationId);
        }

        /// <summary>
        /// Updates the status of an existing application.
        /// </summary>
        /// <param name="userId">The ID of the user updating the application.</param>
        /// <param name="applicationId">The ID of the application to be updated.</param>
        /// <param name="applicationStatusTypeId">The new status type ID to apply to the application.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task UpdateApplicationAsync(string userId, string applicationId, string applicationStatusTypeId)
        {
            if (string.IsNullOrEmpty(applicationId))
            {
                throw new JobApplicationException("Application ID is null or empty.");
            }

            var application = await GetOrCacheApplicationByIdAsync(applicationId);

            if (application == null)
                throw new JobApplicationException(Error_ApplicationNotFound);
            if (application.ApplicationStatusTypeId == applicationStatusTypeId)
                throw new JobApplicationException(Error_ApplicationStatusUnchanged);

            application.ApplicationStatusTypeId = applicationStatusTypeId;
            application.UpdatedDate = DateTime.Now;
            application.Job = null;
            application.User = null;
            application.ApplicationStatusType = null;

            using (var scope = _serviceProvider.CreateScope())
            {
                var repository = scope.ServiceProvider.GetRequiredService<IApplicationRepository>();
                await repository.UpdateApplicationAsync(application);
            }

            await UpdateApplicationCacheAsync(userId, applicationId);
        }

        /// <summary>
        /// Archives a specified application.
        /// </summary>
        /// <param name="userId">The ID of the user requesting the archive.</param>
        /// <param name="applicationId">The ID of the application to be archived.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task ArchiveApplicationAsync(string userId, string applicationId)
        {
            var validStatuses = new HashSet<string> { AppStatus_Withdrawn, AppStatus_Rejected, AppStatus_Accepted };
            if (!validStatuses.Contains(applicationId))
                throw new JobApplicationException(Error_ApplicationCannotArchive);

            var application = await GetOrCacheApplicationByIdAsync(applicationId);
            if (application == null)
                throw new JobApplicationException(Error_ApplicationNotFound);

            application.IsArchived = true;
            application.UpdatedDate = DateTime.Now;

            using (var scope = _serviceProvider.CreateScope())
            {
                var repository = scope.ServiceProvider.GetRequiredService<IApplicationRepository>();
                await repository.UpdateApplicationAsync(application);
            }

            await UpdateApplicationCacheAsync(userId, applicationId);
        }

        /// <summary>
        /// Retrieves a paginated list of all applications based on the provided filters.
        /// </summary>
        /// <param name="filters">The filter criteria for retrieving applications.</param>
        /// <returns>A task representing the asynchronous operation, containing a paginated list of application view models.</returns>
        public async Task<PaginatedList<ApplicationViewModel>> GetAllApplicationsAsync(FilterServiceModel filters)
        {
            var pageIndex = filters.PageIndex;
            var pageSize = filters.PageSize;
            var search = filters.Search;
            var sortBy = filters.SortBy;
            var statusFilter = filters.StatusFilter;
            var workSetupFilter = filters.WorkSetupFilter;
            var programFilter = filters.ProgramFilter;
            var userId = filters.UserId;
            var userRole = filters.UserRole;

            var applications = _mapper.Map<List<ApplicationViewModel>>(await GetOrCacheAllApplicationsAsync());

            applications = userRole switch
            {
                "Admin" => applications,
                "NLO" => applications,
                "Recruiter" => applications.Where(a => a.Job.PostedById == userId).ToList(),
                _ => applications.Where(a => a.ApplicantId == userId).ToList(),
            };

            if (!string.IsNullOrEmpty(search))
            {
                applications = applications.Where(a =>
                    a.Job.Title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    (userRole != Role_Applicant &&
                        (
                            a.ApplicantName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                            a.Applicant.Address.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                            a.Applicant.User.Email.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                            a.Applicant.EducationalDetail.IdNumber.Contains(search, StringComparison.OrdinalIgnoreCase)
                        )
                    ) ||
                    (userRole == Role_Applicant &&
                        (
                            a.RecruiterName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                            a.Job.PostedBy.User.Email.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                            a.Job.PostedBy.Title.Contains(search, StringComparison.OrdinalIgnoreCase)
                        )
                    ) ||
                    (userRole != Role_Recruiter &&
                        (
                            a.Job.Company.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                            a.Job.Company.ContactEmail.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                            a.Job.Company.ContactNumber.Contains(search, StringComparison.OrdinalIgnoreCase)
                        )
                    ) ||
                    a.Job.Location.Contains(search, StringComparison.OrdinalIgnoreCase)
                    ).ToList();
            }

            if (!string.IsNullOrEmpty(statusFilter))
                applications = applications.Where(a => a.ApplicationStatusTypeId == statusFilter).ToList();

            if (!string.IsNullOrEmpty(workSetupFilter))
                applications = applications.Where(a => a.Job.SetupType.SetupTypeId == workSetupFilter).ToList();

            if (!string.IsNullOrEmpty(programFilter))
                applications = applications.Where(a => a.Applicant.EducationalDetail.ProgramId == programFilter).ToList();

            applications = sortBy switch
            {
                "created_desc" => applications.OrderByDescending(a => a.CreatedDate).ToList(),
                "created_asc" => applications.OrderBy(a => a.CreatedDate).ToList(),
                "title_desc" => applications.OrderByDescending(a => a.Job.Title).ToList(),
                "title_asc" => applications.OrderBy(a => a.Job.Title).ToList(),
                _ => applications.OrderByDescending(a => a.CreatedDate).ToList(),
            };

            var totalCount = applications.Count;
            var items = applications.Skip((pageIndex - 1) * pageSize).Take(pageSize);

            return new PaginatedList<ApplicationViewModel>(items, totalCount, pageIndex, pageSize);
        }

        /// <summary>
        /// Retrieves an application by its ID.
        /// </summary>
        /// <param name="id">The ID of the application to retrieve.</param>
        /// <returns>A task representing the asynchronous operation, containing the application view model.</returns>
        public async Task<ApplicationViewModel> GetApplicationByIdAsync(string id) =>
            _mapper.Map<ApplicationViewModel>(await GetOrCacheApplicationByIdAsync(id));

        #region Get or Cache Methods
        /// <summary>
        /// Retrieves all applications from the cache or database, caching the result if necessary.
        /// </summary>
        /// <returns>A task representing the asynchronous operation, containing a list of applications.</returns>
        private async Task<List<Application>> GetOrCacheAllApplicationsAsync()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var cachingService = scope.ServiceProvider.GetRequiredService<ICachingService>();
                return await cachingService.GetOrCacheAsync(Key_ApplicationsAll, _serviceProvider, async (innerScope) =>
                {
                    var repository = innerScope.ServiceProvider.GetRequiredService<IApplicationRepository>();
                    return await repository.GetAllApplicationsAsync(true);
                }, cacheExpirationMinutes);
            }
        }

        /// <summary>
        /// Retrieves all applications for a specific user from the cache or database, caching the result if necessary.
        /// </summary>
        /// <param name="userId">The ID of the user whose applications to retrieve.</param>
        /// <returns>A task representing the asynchronous operation, containing a list of applications.</returns>
        private async Task<List<Application>> GetOrCacheAllApplicationsByUserAsync(string userId)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var cachingService = scope.ServiceProvider.GetRequiredService<ICachingService>();
                string applicationUserKey = string.Format(Key_ApplicationsByUserId, userId);
                return await cachingService.GetOrCacheAsync(applicationUserKey, _serviceProvider, async (innerScope) =>
                {
                    var repository = scope.ServiceProvider.GetRequiredService<IApplicationRepository>();
                    return await repository.GetAllApplicationsByUserAsync(userId);
                }, cacheExpirationMinutes);
            }
        }

        /// <summary>
        /// Retrieves a specific application by its ID from the cache or database, caching the result if necessary.
        /// </summary>
        /// <param name="id">The ID of the application to retrieve.</param>
        /// <returns>A task representing the asynchronous operation, containing the application.</returns>
        private async Task<Application> GetOrCacheApplicationByIdAsync(string id)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var cachingService = scope.ServiceProvider.GetRequiredService<ICachingService>();
                string applicationKey = string.Format(Key_ApplicationById, id);
                return await cachingService.GetOrCacheAsync(applicationKey, _serviceProvider, async (innerScope) =>
                {
                    var repository = scope.ServiceProvider.GetRequiredService<IApplicationRepository>();
                    return await repository.GetApplicationByIdAsync(id);
                }, cacheExpirationMinutes);
            }
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Updates the cache for the specified application and related data.
        /// </summary>
        /// <param name="userId">The ID of the user whose application cache needs updating.</param>
        /// <param name="applicationId">The ID of the application to update in the cache.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task UpdateApplicationCacheAsync(string userId, string applicationId)
        {
            var applicationUserKey = string.Format(Key_ApplicationsByUserId, userId);
            var applicationKey = string.Format(Key_ApplicationById, applicationId);

            await UpdateApplicationsCacheAsync(Key_ApplicationsAll);
            await UpdateUserApplicationsCacheAsync(applicationUserKey, userId);
            await UpdateApplicationCacheByIdAsync(applicationKey, applicationId);
        }

        /// <summary>
        /// Checks if an existing application exists for the specified user and job.
        /// </summary>
        /// <param name="userId">The ID of the user to check for existing applications.</param>
        /// <param name="jobId">The ID of the job to check for existing applications.</param>
        /// <returns>A task representing the asynchronous operation, returning true if an application exists.</returns>
        private async Task<bool> HasExistingApplicationAsync(string userId, string jobId)
        {
            var applications = await GetOrCacheAllApplicationsByUserAsync(userId);
            return applications.Exists(a => a.JobId == jobId);
        }

        /// <summary>
        /// Updates the cache for all applications based on the specified key.
        /// </summary>
        /// <param name="key">The cache key for the applications.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task UpdateApplicationsCacheAsync(string key)
        {
            var threadEvent = new DataListUpdatedEvent<Application>
            {
                Key = key,
                ServiceProvider = _serviceProvider,
                FetchUpdatedData = async (scope) =>
                {
                    var repository = scope.ServiceProvider.GetRequiredService<IApplicationRepository>();
                    return await repository.GetAllApplicationsAsync(true);
                },
                ExpirationMinutes = cacheExpirationMinutes
            };
            _eventBus.Publish(threadEvent);

            await Task.CompletedTask;
        }

        /// <summary>
        /// Updates the cache for applications associated with a specific user based on the specified key.
        /// </summary>
        /// <param name="key">The cache key for the user's applications.</param>
        /// <param name="userId">The ID of the user whose application cache needs updating.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task UpdateUserApplicationsCacheAsync(string key, string userId)
        {
            var threadEvent = new DataListUpdatedEvent<Application>
            {
                Key = key,
                ServiceProvider = _serviceProvider,
                FetchUpdatedData = async (scope) =>
                {
                    var repository = scope.ServiceProvider.GetRequiredService<IApplicationRepository>();
                    return await repository.GetAllApplicationsByUserAsync(userId);
                },
                ExpirationMinutes = cacheExpirationMinutes
            };
            _eventBus.Publish(threadEvent);

            await Task.CompletedTask;
        }

        /// <summary>
        /// Updates the cache for a specific application based on its ID and cache key.
        /// </summary>
        /// <param name="key">The cache key for the application.</param>
        /// <param name="applicationId">The ID of the application to update in the cache.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task UpdateApplicationCacheByIdAsync(string key, string applicationId)
        {
            var threadEvent = new DataUpdatedEvent<Application>
            {
                Key = key,
                ServiceProvider = _serviceProvider,
                FetchUpdatedData = async (scope) =>
                {
                    var repository = scope.ServiceProvider.GetRequiredService<IApplicationRepository>();
                    return await repository.GetApplicationByIdAsync(applicationId);
                },
                ExpirationMinutes = cacheExpirationMinutes
            };
            _eventBus.Publish(threadEvent);

            await Task.CompletedTask;
        }
        #endregion
    }
}
