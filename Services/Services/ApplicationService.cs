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


namespace Services.Services
{
    /// <summary>
    /// Service class for handling operations related to teams.
    /// </summary>
    public class ApplicationService : IApplicationService
    {
        private readonly IApplicationRepository _repository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public ApplicationService(
            IApplicationRepository repository,
            IMapper mapper,
            ILogger<ApplicationService> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
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
            //application.IsArchived = 

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

        public async Task<PaginatedList<ApplicationViewModel>> GetApplicationsAsync(
        int pageIndex, int pageSize, string search, string statusFilter)
        {
            var userId = _httpContextAccessor.HttpContext.User.Identity.Name; 

            var query = _repository.GetApplicationWithIncludes()
                .Where(app => app.UserId == userId);

            if (!string.IsNullOrEmpty(search))
                query = query.Where(app => app.JobId.Contains(search));

            if (!string.IsNullOrEmpty(statusFilter))
                query = query.Where(app => app.ApplicationStatusTypeId == statusFilter);

            var totalCount = await query.CountAsync();
            var applications = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var viewModel = _mapper.Map<List<ApplicationViewModel>>(applications);

            return new PaginatedList<ApplicationViewModel>(viewModel, totalCount, pageIndex, pageSize);
        }

    }
}
