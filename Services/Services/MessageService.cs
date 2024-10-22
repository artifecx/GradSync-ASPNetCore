using Data.Models;
using Data.Interfaces;
using Services.Interfaces;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.Extensions.Caching.Memory;
using System.Threading;

namespace Services.Services 
{
    public class MessageService : IMessageService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IMemoryCache _memoryCache;

        public MessageService(IServiceProvider serviceProvider, IMemoryCache memoryCache)
        {
            _serviceProvider = serviceProvider;
            _memoryCache = memoryCache;
        }

        public async Task AddMessageAsync(Message message) 
        {
            var messageKey = $"RecentMessages-{message.MessageThreadId}";
            var threadKey = $"Threads-{message.MessageThreadId}";

            using (var scope = _serviceProvider.CreateScope())
            {
                var repository = scope.ServiceProvider.GetRequiredService<IMessageRepository>();
                await repository.AddMessageAsync(message);
            }

            InvalidateCache(messageKey);
            InvalidateCache(threadKey);

            await UpdateCacheAsync(messageKey, async (scope) =>
            {
                var repository = scope.ServiceProvider.GetRequiredService<IMessageRepository>();
                return await repository.GetRecentMessagesAsync(message.MessageThreadId);
            });

            await UpdateCacheAsync(threadKey, async (scope) =>
            {
                var repository = scope.ServiceProvider.GetRequiredService<IMessageRepository>();
                return await repository.GetThreadByIdAsync(message.MessageThreadId);
            });
        }

        public async Task CreateMessageThreadAsync(MessageThread thread)
        {
            var key = $"Threads-{thread.MessageThreadId}";
            using (var scope = _serviceProvider.CreateScope())
            {
                var repository = scope.ServiceProvider.GetRequiredService<IMessageRepository>();
                await repository.CreateThreadAsync(thread);
            }
            InvalidateCache(key);
            await UpdateCacheAsync(key, async (scope) =>
            {
                var repository = scope.ServiceProvider.GetRequiredService<IMessageRepository>();
                return await repository.GetThreadByIdAsync(thread.MessageThreadId);
            });
        }

        public async Task<List<Message>> GetRecentMessagesAsync(string threadId) =>
            await GetOrSetCacheAsync($"RecentMessages-{threadId}", async (scope) =>
            {
                var repository = scope.ServiceProvider.GetRequiredService<IMessageRepository>();
                return await repository.GetRecentMessagesAsync(threadId);
            });

        public async Task<MessageThread> GetMessageThreadByIdAsync(string threadId) =>
            await GetOrSetCacheAsync($"Threads-{threadId}", async (scope) =>
            {
                var repository = scope.ServiceProvider.GetRequiredService<IMessageRepository>();
                return await repository.GetThreadByIdAsync(threadId);
            });

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

        private async Task<T> GetOrSetCacheAsync<T>(string cacheKey, Func<IServiceScope, Task<T>> getDataFunc)
        {
            if (!_memoryCache.TryGetValue(cacheKey, out T cachedData))
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    cachedData = await getDataFunc(scope);
                    _memoryCache.Set(cacheKey, cachedData);
                }
            }
            return cachedData;
        }

        public void InvalidateCache(string cacheKey)
        {
            if (string.IsNullOrEmpty(cacheKey))
            {
                throw new ArgumentException("Cache key cannot be null or empty", nameof(cacheKey));
            }

            _memoryCache.Remove(cacheKey);
        }

        public async Task UpdateCacheAsync<T>(string cacheKey, Func<IServiceScope, Task<List<T>>> getDataFunc)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var freshData = await getDataFunc(scope);
                _memoryCache.Set(cacheKey, freshData);
            }
        }

        public async Task UpdateCacheAsync<T>(string cacheKey, Func<IServiceScope, Task<T>> getDataFunc)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var freshData = await getDataFunc(scope);
                _memoryCache.Set(cacheKey, freshData);
            }
        }
    }
}