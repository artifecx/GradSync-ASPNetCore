using Data.Models;
using Microsoft.AspNetCore.SignalR;
using Services.Interfaces;
using System;
using System.Threading.Tasks;
using Humanizer;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace WebApp.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IMessageService _messageService;

        public ChatHub(IMessageService messageService)
        {
            _messageService = messageService;
        }

        public async Task SendMessageToThread(string threadId, string userId1, string userId2, string content, string title)
        {
            var userName = Context.User.Identity.Name;
            var userId = Context.UserIdentifier;

            var thread = await _messageService.GetMessageThreadByIdAsync(threadId);

            if (thread == null)
            {
                thread = await CreatePrivateThread(threadId, title, userId1, userId2);
                await Groups.AddToGroupAsync(Context.ConnectionId, threadId.ToString());
            }
            var isParticipant = thread.MessageParticipants.Any(p => p.UserId == userId);

            if (!isParticipant)
            {
                throw new UnauthorizedAccessException("You are not a participant in this thread.");
            }

            var message = new Message
            {
                MessageId = Guid.NewGuid().ToString(),
                MessageThreadId = threadId,
                UserId = userId,
                Content = content,
                Timestamp = DateTime.Now
            };

            await _messageService.AddMessageAsync(message);
            await Clients.Group(threadId).SendAsync("ReceiveMessage", userName, content, message.Timestamp, userId);
        }

        public async Task<MessageThread> CreatePrivateThread(string threadId, string title, string userId1, string userId2)
        {
            var thread = new MessageThread
            {
                MessageThreadId = threadId,
                Title = title,
                MessageParticipants = new List<MessageParticipant>
                {
                    new MessageParticipant { MessageParticipantId = Guid.NewGuid().ToString(), UserId = userId1 },
                    new MessageParticipant { MessageParticipantId = Guid.NewGuid().ToString(), UserId = userId2 }
                }
            };
            await _messageService.CreateMessageThreadAsync(thread);
            return thread;
        }

        public async Task<List<Message>> GetThreadMessages(string threadId)
        {
            if (string.IsNullOrEmpty(threadId)) return new List<Message>();

            var thread = await _messageService.GetMessageThreadByIdAsync(threadId);

            if (thread == null) return new List<Message>();

            var currentUserId = Context.UserIdentifier;
            bool isAdmin = Context.User.IsInRole("Admin") || Context.User.IsInRole("NLO");

            if (isAdmin || thread.MessageParticipants.Any(p => p.MessageThreadId == threadId && p.UserId == currentUserId))
            {
                return thread.Messages.Select(m => new Message
                {
                    Content = m.Content,
                    Timestamp = m.Timestamp,
                    UserId = m.UserId,
                    User = new User
                    {
                        UserId = m.User.UserId,
                        FirstName = m.User.FirstName
                    }   
                }).OrderBy(m => m.Timestamp).ToList();
            }
            else
            {
                throw new UnauthorizedAccessException("You are not a participant in this thread.");
            }
        }

        public override async Task OnConnectedAsync()
        {
            var threadId = Context.GetHttpContext().Request.Query["threadId"];
            var userId = Context.UserIdentifier;

            var thread = await _messageService.GetMessageThreadByIdAsync(threadId);

            if (thread == null) return;

            var isParticipant = thread.MessageParticipants.Any(p => p.UserId == userId);

            if (isParticipant)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, threadId.ToString());
            }

            await base.OnConnectedAsync();
        }
    }
}