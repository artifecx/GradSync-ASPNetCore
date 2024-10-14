using Services.Interfaces;
using Services.ServiceModels;
using System;
using System.Collections.Concurrent;


namespace Services.Services
{
    public class EmailQueue : IEmailQueue
    {
        private readonly ConcurrentQueue<EmailMessage> _emails = new ConcurrentQueue<EmailMessage>();

        public void EnqueueEmail(EmailMessage emailMessage)
        {
            if (emailMessage == null)
                throw new ArgumentNullException(nameof(emailMessage));

            _emails.Enqueue(emailMessage);
        }

        public bool TryDequeue(out EmailMessage emailMessage)
        {
            return _emails.TryDequeue(out emailMessage);
        }
    }
}
