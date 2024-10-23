using Data.Models;
using Data.Interfaces;
using Services.Interfaces;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.Extensions.Caching.Memory;
using Services.ServiceModels;
using Services.EventBus;
using static Resources.Constants.ExpirationTimes;

namespace Services.Services 
{
    public class MessageService : IMessageService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IMemoryCache _memoryCache;
        private readonly IEventBus _eventBus;
        private static TimeSpan cacheExpirationMinutes = 
            TimeSpan.FromMinutes(Convert.ToInt32(Expiration_Messages));

        public MessageService(IServiceProvider serviceProvider, IMemoryCache memoryCache, IEventBus eventBus)
        {
            _serviceProvider = serviceProvider;
            _memoryCache = memoryCache;
            _eventBus = eventBus;
        }

        public async Task AddMessageAsync(Message message)
        {
            var messageKey = $"RecentMessages-{message.MessageThreadId}";
            var threadKey = $"Thread-{message.MessageThreadId}";

            using (var scope = _serviceProvider.CreateScope())
            {
                var repository = scope.ServiceProvider.GetRequiredService<IMessageRepository>();
                await repository.AddMessageAsync(message);
            }

            var recentMessagesEvent = new DataListUpdatedEvent<Message>
            {
                Key = messageKey,
                Cache = _memoryCache,
                ServiceProvider = _serviceProvider,
                FetchUpdatedData = async (scope) =>
                {
                    var repository = scope.ServiceProvider.GetRequiredService<IMessageRepository>();
                    return await repository.GetRecentMessagesAsync(message.MessageThreadId);
                },
                ExpirationMinutes = cacheExpirationMinutes
            };
            _eventBus.Publish(recentMessagesEvent);

            var threadEvent = new DataUpdatedEvent<MessageThread>
            {
                Key = threadKey,
                Cache = _memoryCache,
                ServiceProvider = _serviceProvider,
                FetchUpdatedData = async (scope) =>
                {
                    var repository = scope.ServiceProvider.GetRequiredService<IMessageRepository>();
                    return await repository.GetThreadByIdAsync(message.MessageThreadId);
                },
                ExpirationMinutes = cacheExpirationMinutes
            };
            _eventBus.Publish(threadEvent);
        }

        public async Task CreateMessageThreadAsync(MessageThread thread)
        {
            var key = $"Thread-{thread.MessageThreadId}";
            using (var scope = _serviceProvider.CreateScope())
            {
                var repository = scope.ServiceProvider.GetRequiredService<IMessageRepository>();
                await repository.CreateThreadAsync(thread);
            }

            var threadEvent = new DataUpdatedEvent<MessageThread>
            {
                Key = key,
                Cache = _memoryCache,
                ServiceProvider = _serviceProvider,
                FetchUpdatedData = async (scope) =>
                {
                    var repository = scope.ServiceProvider.GetRequiredService<IMessageRepository>();
                    return await repository.GetThreadByIdAsync(thread.MessageThreadId);
                },
                ExpirationMinutes = cacheExpirationMinutes
            };
            _eventBus.Publish(threadEvent);
        }

        public async Task<List<Message>> GetRecentMessagesAsync(string threadId)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var cachingService = scope.ServiceProvider.GetRequiredService<ICachingService>();
                return await cachingService.GetOrCacheAsync($"RecentMessages-{threadId}", _memoryCache, _serviceProvider, async (innerScope) =>
                {
                    var repository = innerScope.ServiceProvider.GetRequiredService<IMessageRepository>();
                    return await repository.GetRecentMessagesAsync(threadId);
                }, cacheExpirationMinutes);
            }
        }

        public async Task<MessageThread> GetMessageThreadByIdAsync(string threadId) 
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var cachingService = scope.ServiceProvider.GetRequiredService<ICachingService>();
                return await cachingService.GetOrCacheAsync($"Thread-{threadId}", _memoryCache, _serviceProvider, async (innerScope) =>
                {
                    var repository = innerScope.ServiceProvider.GetRequiredService<IMessageRepository>();
                    return await repository.GetThreadByIdAsync(threadId);
                }, cacheExpirationMinutes);
            }
        }
    }
}