using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly.Retry;
using Polly;
using Services.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Services.Services
{
    public class EmailBackgroundService : BackgroundService
    {
        private readonly IEmailQueue _emailQueue;
        private readonly IEmailService _emailService;
        private readonly ILogger<EmailBackgroundService> _logger;
        private readonly TimeSpan _delay = TimeSpan.FromSeconds(5);

        private readonly AsyncRetryPolicy _retryPolicy;

        public EmailBackgroundService(IEmailQueue emailQueue, IEmailService emailService, ILogger<EmailBackgroundService> logger)
        {
            _emailQueue = emailQueue;
            _emailService = emailService;
            _logger = logger;

            _retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                    onRetry: (exception, timeSpan, retryCount, context) =>
                    {
                        _logger.LogWarning(exception, "Retry {RetryCount} after {TimeSpan} due to error.", retryCount, timeSpan);
                    });
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Email Background Service is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (_emailQueue.TryDequeue(out var emailMessage))
                    {
                        _logger.LogInformation("Sending email to {ToEmail}", emailMessage.ToEmail);
                        await _retryPolicy.ExecuteAsync(async () =>
                        {
                            await _emailService.SendEmailAsync(emailMessage.ToEmail, emailMessage.Subject, emailMessage.Body);
                        });
                        _logger.LogInformation("Email sent to {ToEmail}", emailMessage.ToEmail);
                    }
                    else
                    {
                        await Task.Delay(_delay, stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while sending an email.");
                    await Task.Delay(_delay, stoppingToken); // Wait before retrying
                }
            }

            _logger.LogInformation("Email Background Service is stopping.");
        }
    }
}