using Data.Models;
using Data.Interfaces;
using Services.Interfaces;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.Extensions.Caching.Memory;
using Services.EventBus;
using Services.ServiceModels;
using System.Threading;

namespace Services.Services
{
    public class CachingService : ICachingService
    {
        private static readonly SemaphoreSlim CacheLock = new SemaphoreSlim(1, 1);
        public CachingService(IEventBus eventBus)
        {
            eventBus.Subscribe<DataUpdatedEvent<MessageThread>>(OnDataUpdated);
            eventBus.Subscribe<DataUpdatedEvent<Application>>(OnDataUpdated);
            eventBus.Subscribe<DataListUpdatedEvent<Message>>(OnDataListUpdated);
            eventBus.Subscribe<DataListUpdatedEvent<Application>>(OnDataListUpdated);
        }

        private static async void OnDataUpdated<T>(DataUpdatedEvent<T> @event)
        {
            await RefreshCacheAsync(@event.Key, @event.Cache, @event.ServiceProvider, @event.FetchUpdatedData, @event.ExpirationMinutes);
        }

        private static async void OnDataListUpdated<T>(DataListUpdatedEvent<T> @event)
        {
            await RefreshCacheAsync(@event.Key, @event.Cache, @event.ServiceProvider, @event.FetchUpdatedData, @event.ExpirationMinutes);
        }

        private static async void OnDataDeleted(DataDeletedEvent @event)
        {
            await InvalidateCacheAsync(@event.Key, @event.Cache);
        }

        public async Task<List<T>> GetOrCacheAsync<T>(
            string cacheKey,
            IMemoryCache cache,
            IServiceProvider serviceProvider,
            Func<IServiceScope, Task<List<T>>> getDataFunc,
            TimeSpan? cacheExpiration = null)
        {
            if (!cache.TryGetValue(cacheKey, out List<T> cachedData))
            {
                await CacheLock.WaitAsync();
                try
                {
                    if (!cache.TryGetValue(cacheKey, out cachedData))
                    {
                        using (var scope = serviceProvider.CreateScope())
                        {
                            cachedData = await getDataFunc(scope);
                            if (EqualityComparer<List<T>>.Default.Equals(cachedData, default) || cachedData.Count == 0)
                                return default;

                            var cacheEntryOptions = new MemoryCacheEntryOptions();
                            if (cacheExpiration.HasValue)
                                cacheEntryOptions.SetAbsoluteExpiration(cacheExpiration.Value);

                            cache.Set(cacheKey, cachedData, cacheEntryOptions);
                        }
                    }
                }
                finally
                {
                    CacheLock.Release(); 
                }
            }
            return cachedData;
        }

        public async Task<T> GetOrCacheAsync<T>(
            string cacheKey,
            IMemoryCache cache,
            IServiceProvider serviceProvider,
            Func<IServiceScope, Task<T>> getDataFunc,
            TimeSpan? cacheExpiration = null)
        {
            if (!cache.TryGetValue(cacheKey, out T cachedData))
            {
                await CacheLock.WaitAsync();
                try
                {
                    if (!cache.TryGetValue(cacheKey, out cachedData))
                    {
                        using (var scope = serviceProvider.CreateScope())
                        {
                            cachedData = await getDataFunc(scope);
                            if (EqualityComparer<T>.Default.Equals(cachedData, default))
                                return default;

                            var cacheEntryOptions = new MemoryCacheEntryOptions();
                            if (cacheExpiration.HasValue)
                                cacheEntryOptions.SetAbsoluteExpiration(cacheExpiration.Value);

                            cache.Set(cacheKey, cachedData, cacheEntryOptions);
                        }
                    }
                }
                finally
                {
                    CacheLock.Release(); 
                }
            }
            return cachedData;
        }

        private static async Task RefreshCacheAsync<T>(
            string cacheKey,
            IMemoryCache cache, 
            IServiceProvider serviceProvider, 
            Func<IServiceScope, Task<List<T>>> getDataFunc,
            TimeSpan? cacheExpiration = null)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var freshData = await getDataFunc(scope);

                var cacheEntryOptions = new MemoryCacheEntryOptions();
                if (cacheExpiration.HasValue)
                    cacheEntryOptions.SetAbsoluteExpiration(cacheExpiration.Value);

                cache.Set(cacheKey, freshData, cacheEntryOptions);
            }
        }

        private static async Task RefreshCacheAsync<T>(
            string cacheKey,
            IMemoryCache cache,
            IServiceProvider serviceProvider, 
            Func<IServiceScope, Task<T>> getDataFunc,
            TimeSpan? cacheExpiration = null)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var freshData = await getDataFunc(scope);

                var cacheEntryOptions = new MemoryCacheEntryOptions();
                if (cacheExpiration.HasValue)
                    cacheEntryOptions.SetAbsoluteExpiration(cacheExpiration.Value);

                cache.Set(cacheKey, freshData, cacheEntryOptions);
            }
        }

        private static Task InvalidateCacheAsync(string key, IMemoryCache cache)
        {
            cache.Remove(key);
            return Task.CompletedTask;
        }
    }
}