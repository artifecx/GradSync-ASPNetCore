using Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IMessageService
    {
        Task AddMessageAsync(Message message);
        Task CreateMessageThreadAsync(MessageThread thread);
        Task<List<Message>> GetRecentMessagesOfThreadAsync(string threadId);
        Task<MessageThread> GetMessageThreadByIdAsync(string threadId);
    }
}
