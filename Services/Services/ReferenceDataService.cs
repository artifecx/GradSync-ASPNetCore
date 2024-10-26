using Data.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Services.Interfaces;
using Data.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;

namespace Services.Services
{
    /// <summary>
    /// Service class for handling reference data.
    /// </summary>
    public class ReferenceDataService : IReferenceDataService
    {
        private readonly IServiceProvider _serviceProvider;

        public ReferenceDataService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        #region Get Methods        
        public async Task<List<EmploymentType>> GetEmploymentTypesAsync()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var cachingService = scope.ServiceProvider.GetRequiredService<ICachingService>();
                return await cachingService.GetOrCacheAsync("EmploymentTypes", _serviceProvider, async (innerScope) =>
                {
                    var repository = innerScope.ServiceProvider.GetRequiredService<IReferenceDataRepository>();
                    return await repository.GetEmploymentTypesAsync();
                });
            }
        }

        public async Task<List<StatusType>> GetStatusTypesAsync() 
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var cachingService = scope.ServiceProvider.GetRequiredService<ICachingService>();
                return await cachingService.GetOrCacheAsync("StatusTypes", _serviceProvider, async (innerScope) =>
                {
                    var repository = innerScope.ServiceProvider.GetRequiredService<IReferenceDataRepository>();
                    return await repository.GetStatusTypesAsync();
                });
            }
        }

        public async Task<List<SetupType>> GetWorkSetupsAsync()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var cachingService = scope.ServiceProvider.GetRequiredService<ICachingService>();
                return await cachingService.GetOrCacheAsync("WorkSetups", _serviceProvider, async (innerScope) =>
                {
                    var repository = innerScope.ServiceProvider.GetRequiredService<IReferenceDataRepository>();
                    return await repository.GetWorkSetupsAsync();
                });
            }
        }

        public async Task<List<Program>> GetProgramsAsync()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var cachingService = scope.ServiceProvider.GetRequiredService<ICachingService>();
                return await cachingService.GetOrCacheAsync("Programs", _serviceProvider, async (innerScope) =>
                {
                    var repository = innerScope.ServiceProvider.GetRequiredService<IReferenceDataRepository>();
                    return await repository.GetProgramsAsync();
                });
            }
        }

        public async Task<List<Skill>> GetSkillsAsync()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var cachingService = scope.ServiceProvider.GetRequiredService<ICachingService>();
                return await cachingService.GetOrCacheAsync("Skills", _serviceProvider, async (innerScope) =>
                {
                    var repository = innerScope.ServiceProvider.GetRequiredService<IReferenceDataRepository>();
                    return await repository.GetSkillsAsync();
                });
            }
        }

        public async Task<List<YearLevel>> GetYearLevelsAsync()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var cachingService = scope.ServiceProvider.GetRequiredService<ICachingService>();
                return await cachingService.GetOrCacheAsync("YearLevels", _serviceProvider, async (innerScope) =>
                {
                    var repository = innerScope.ServiceProvider.GetRequiredService<IReferenceDataRepository>();
                    return await repository.GetYearLevelsAsync();
                });
            }
        }

        public async Task<List<ApplicationStatusType>> GetApplicationStatusTypesAsync()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var cachingService = scope.ServiceProvider.GetRequiredService<ICachingService>();
                return await cachingService.GetOrCacheAsync("AppStatusTypes", _serviceProvider, async (innerScope) =>
                {
                    var repository = innerScope.ServiceProvider.GetRequiredService<IReferenceDataRepository>();
                    return await repository.GetApplicationStatusTypesAsync();
                });
            }
        }

        public async Task<List<Role>> GetUserRolesAsync()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var cachingService = scope.ServiceProvider.GetRequiredService<ICachingService>();
                return await cachingService.GetOrCacheAsync("UserRoles", _serviceProvider, async (innerScope) =>
                {
                    var repository = innerScope.ServiceProvider.GetRequiredService<IReferenceDataRepository>();
                    return await repository.GetUserRolesAsync();
                });
            }
        }
        #endregion
    }
}
