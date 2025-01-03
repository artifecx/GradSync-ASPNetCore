﻿using Data.Models;
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
        Task<List<T>> GetOrCacheAsync<T>(string cacheKey, IServiceProvider serviceProvider, 
            Func<IServiceScope, Task<List<T>>> getDataFunc, TimeSpan? cacheExpiration = null);
        Task<T> GetOrCacheAsync<T>(string cacheKey, IServiceProvider serviceProvider, 
            Func<IServiceScope, Task<T>> getDataFunc, TimeSpan? cacheExpiration = null);
    }
}
