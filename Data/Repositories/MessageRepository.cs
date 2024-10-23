using Data.Interfaces;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class MessageRepository : BaseRepository, IMessageRepository
    {
        public MessageRepository(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        public async Task AddMessageAsync(Message message)
        {
            this.GetDbSet<Message>().Add(message);
            await UnitOfWork.SaveChangesAsync();
        }

        public async Task UpdateMessageAsync(Message message)
        {
            this.GetDbSet<Message>().Update(message);
            await UnitOfWork.SaveChangesAsync();
        }

        public async Task CreateThreadAsync(MessageThread thread)
        {
            this.GetDbSet<MessageThread>().Add(thread);
            await UnitOfWork.SaveChangesAsync();
        }

        public async Task<List<Message>> GetRecentMessagesAsync(string threadId) => 
            await this.GetDbSet<Message>()
                .Where(m => m.MessageThreadId == threadId)
                .Include(m => m.User)
                .Select(m => new Message
                {
                    MessageId = m.MessageId,
                    MessageThreadId = m.MessageThreadId,
                    UserId = m.UserId,
                    Content = m.Content,
                    Timestamp = m.Timestamp,
                    User = new User
                    {
                        UserId = m.User.UserId,
                        FirstName = m.User.FirstName
                    }
                })
                .OrderByDescending(m => m.Timestamp)
                .Take(50)
                .AsNoTracking()
                .ToListAsync();

        public async Task<MessageThread> GetThreadByIdAsync(string threadId) =>
            await this.GetDbSet<MessageThread>()
                .Include(t => t.Messages)
                    .ThenInclude(m => m.User)
                .Include(t => t.MessageParticipants)
                .Select(t => new MessageThread
                {
                    MessageThreadId = t.MessageThreadId,
                    Title = t.Title,
                    Messages = t.Messages.Select(m => new Message
                    {
                        MessageId = m.MessageId,
                        MessageThreadId = m.MessageThreadId,
                        UserId = m.UserId,
                        Content = m.Content,
                        Timestamp = m.Timestamp,
                        User = new User
                        {
                            UserId = m.User.UserId,
                            FirstName = m.User.FirstName
                        }
                    })
                    .OrderByDescending(m => m.Timestamp)
                    .Take(50)
                    .ToList(),
                    MessageParticipants = t.MessageParticipants
                })
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.MessageThreadId == threadId);
    }
}
