using Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Data.Interfaces
{
    public interface IMessageRepository
    {
        Task AddMessageAsync(Message message);
        Task CreateThreadAsync(MessageThread thread);
        Task<List<Message>> GetRecentMessagesAsync(string threadId);
        Task<MessageThread> GetThreadByIdAsync(string threadId);
    }   
}
