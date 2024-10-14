using Services.ServiceModels;

namespace Services.Interfaces
{
    public interface IEmailQueue
    {
        void EnqueueEmail(EmailMessage emailMessage);
        bool TryDequeue(out EmailMessage emailMessage);
    }
}
