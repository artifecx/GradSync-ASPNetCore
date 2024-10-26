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
using System.Linq;

namespace Services.Services 
{
    public class MessageService : IMessageService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IEventBus _eventBus;
        private static TimeSpan cacheExpirationMinutes = 
            TimeSpan.FromMinutes(Convert.ToInt32(Expiration_Messages));

        public MessageService(IServiceProvider serviceProvider, IEventBus eventBus)
        {
            _serviceProvider = serviceProvider;
            _eventBus = eventBus;
        }

        public async Task AddMessageAsync(Message message)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var repository = scope.ServiceProvider.GetRequiredService<IMessageRepository>();
                await repository.AddMessageAsync(message);
            }

            var messageKey = $"RecentMessages-{message.MessageThreadId}";
            var threadKey = $"Thread-{message.MessageThreadId}";
            var threadId = message.MessageThreadId;

            await UpdateThreadCacheByIdAsync(threadKey, threadId);
            await UpdateRecentMessagesCacheByIdAsync(messageKey, threadId);
        }

        public async Task CreateMessageThreadAsync(MessageThread thread)
        {
            var threadKey = $"Thread-{thread.MessageThreadId}";
            using (var scope = _serviceProvider.CreateScope())
            {
                var repository = scope.ServiceProvider.GetRequiredService<IMessageRepository>();
                await repository.CreateThreadAsync(thread);
            }

            await UpdateThreadCacheByIdAsync(threadKey, thread.MessageThreadId);
        }

        public async Task<List<Message>> GetRecentMessagesOfThreadAsync(string threadId)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var cachingService = scope.ServiceProvider.GetRequiredService<ICachingService>();
                return await cachingService.GetOrCacheAsync($"RecentMessages-{threadId}", _serviceProvider, async (innerScope) =>
                {
                    var repository = scope.ServiceProvider.GetRequiredService<IMessageRepository>();
                    return await repository.GetRecentMessagesAsync(threadId);
                }, cacheExpirationMinutes);
            }
        }

        public async Task<MessageThread> GetMessageThreadByIdAsync(string threadId) 
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var cachingService = scope.ServiceProvider.GetRequiredService<ICachingService>();
                return await cachingService.GetOrCacheAsync($"Thread-{threadId}", _serviceProvider, async (innerScope) =>
                {
                    var repository = innerScope.ServiceProvider.GetRequiredService<IMessageRepository>();
                    return await repository.GetThreadByIdAsync(threadId);
                }, cacheExpirationMinutes);
            }
        }

        private async Task UpdateRecentMessagesCacheByIdAsync(string messageKey, string threadId)
        {
            var recentMessagesEvent = new DataListUpdatedEvent<Message>
            {
                Key = messageKey,
                ServiceProvider = _serviceProvider,
                FetchUpdatedData = async (scope) =>
                {
                    var repository = scope.ServiceProvider.GetRequiredService<IMessageRepository>();
                    return await repository.GetRecentMessagesAsync(threadId);
                },
                ExpirationMinutes = cacheExpirationMinutes
            };
            _eventBus.Publish(recentMessagesEvent);
            await Task.CompletedTask;
        }

        private async Task UpdateThreadCacheByIdAsync(string threadKey, string threadId)
        {
            var threadEvent = new DataUpdatedEvent<MessageThread>
            {
                Key = threadKey,
                ServiceProvider = _serviceProvider,
                FetchUpdatedData = async (scope) =>
                {
                    var repository = scope.ServiceProvider.GetRequiredService<IMessageRepository>();
                    return await repository.GetThreadByIdAsync(threadId);
                },
                ExpirationMinutes = cacheExpirationMinutes
            };
            _eventBus.Publish(threadEvent);
            await Task.CompletedTask;
        }
    }
}