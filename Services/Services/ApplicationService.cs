using AutoMapper;
using Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Services.Interfaces;
using Services.ServiceModels;
using System;
using System.Threading.Tasks;
using Data.Interfaces;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using static Resources.Constants.UserRoles;

namespace Services.Services
{
    /// <summary>
    /// Service class for handling operations related to teams.
    /// </summary>
    public class ApplicationService : IApplicationService
    {
        private readonly IApplicationRepository _repository;
        private readonly IMapper _mapper;

        public ApplicationService(IApplicationRepository repository, IMapper mapper, ILogger<ApplicationService> logger)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task AddApplicationAsync(ApplicationViewModel model)
        {
            var application = _mapper.Map<Application>(model);
            application.ApplicationId = Guid.NewGuid().ToString();      
            application.ApplicationStatusTypeId = "Submitted";
            application.UserId = Guid.NewGuid().ToString();
            application.JobId = Guid.NewGuid().ToString();
            application.CreatedDate = DateTime.Now;
            application.UpdatedDate = DateTime.Now;
            application.AdditionalInformationId = Guid.NewGuid().ToString();

            await _repository.AddApplicationAsync(application);
        }

        public async Task UpdateApplicationAsync(ApplicationViewModel model)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteApplicationAsync(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<PaginatedList<ApplicationViewModel>> GetAllApplicationsAsync(ApplicationFilter filters)
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

            var applications = _mapper.Map<List<ApplicationViewModel>>(await _repository.GetAllApplications(true));
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

        public async Task<List<ApplicationStatusType>> GetApplicationStatusTypesAsync() =>
            await _repository.GetApplicationStatusTypesAsync();
    }
}
