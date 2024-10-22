using Data.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface ICachingService
    {
        Task<List<T>> GetOrCacheAsync<T>(string cacheKey, IMemoryCache cache, 
            IServiceProvider serviceProvider, Func<IServiceScope, Task<List<T>>> getDataFunc);
        Task<T> GetOrCacheAsync<T>(string cacheKey, IMemoryCache cache, 
            IServiceProvider serviceProvider, Func<IServiceScope, Task<T>> getDataFunc);
    }
}
