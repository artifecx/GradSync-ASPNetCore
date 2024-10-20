using Data.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Services.Interfaces;
using Data.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Services.Services
{
    /// <summary>
    /// Service class for handling reference data.
    /// </summary>
    public class ReferenceDataService : IReferenceDataService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IMemoryCache _memoryCache;

        public ReferenceDataService(IServiceProvider serviceProvider, IMemoryCache memoryCache)
        {
            _serviceProvider = serviceProvider;
            _memoryCache = memoryCache;
        }

        #region Get Methods        
        public async Task<List<EmploymentType>> GetEmploymentTypesAsync() =>
            await GetOrSetCacheAsync("EmploymentTypes", async (scope) =>
            {
                var repository = scope.ServiceProvider.GetRequiredService<IReferenceDataRepository>();
                return await repository.GetEmploymentTypesAsync();
            });

        public async Task<List<StatusType>> GetStatusTypesAsync() =>
            await GetOrSetCacheAsync("StatusTypes", async (scope) =>
            {
                var repository = scope.ServiceProvider.GetRequiredService<IReferenceDataRepository>();
                return await repository.GetStatusTypesAsync();
            });

        public async Task<List<SetupType>> GetWorkSetupsAsync() =>
            await GetOrSetCacheAsync("WorkSetups", async (scope) =>
            {
                var repository = scope.ServiceProvider.GetRequiredService<IReferenceDataRepository>();
                return await repository.GetWorkSetupsAsync();
            });

        public async Task<List<Program>> GetProgramsAsync() =>
            await GetOrSetCacheAsync("Programs", async (scope) =>
            {
                var repository = scope.ServiceProvider.GetRequiredService<IReferenceDataRepository>();
                return await repository.GetProgramsAsync();
            });

        public async Task<List<Skill>> GetSkillsAsync() =>
            await GetOrSetCacheAsync("Skills", async (scope) =>
            {
                var repository = scope.ServiceProvider.GetRequiredService<IReferenceDataRepository>();
                return await repository.GetSkillsAsync();
            });

        public async Task<List<YearLevel>> GetYearLevelsAsync() =>
            await GetOrSetCacheAsync("YearLevels", async (scope) =>
            {
                var repository = scope.ServiceProvider.GetRequiredService<IReferenceDataRepository>();
                return await repository.GetYearLevelsAsync();
            });

        public async Task<List<ApplicationStatusType>> GetApplicationStatusTypesAsync() =>
            await GetOrSetCacheAsync("AppStatusTypes", async (scope) =>
            {
                var repository = scope.ServiceProvider.GetRequiredService<IReferenceDataRepository>();
                return await repository.GetApplicationStatusTypesAsync();
            });
        #endregion Get Methods

        private async Task<List<T>> GetOrSetCacheAsync<T>(string cacheKey, Func<IServiceScope, Task<List<T>>> getDataFunc)
        {
            if (!_memoryCache.TryGetValue(cacheKey, out List<T> cachedData))
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    cachedData = await getDataFunc(scope);
                    _memoryCache.Set(cacheKey, cachedData);
                }
            }
            return cachedData;
        }
    }
}
