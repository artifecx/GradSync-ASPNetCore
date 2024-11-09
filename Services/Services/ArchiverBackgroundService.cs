using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Data.Interfaces;
using System.Linq;
using static Resources.Constants.Types;
using System.Collections.Generic;

namespace Services.Services
{
    public class ArchiverBackgroundService : BackgroundService
    {
        private readonly ILogger<ArchiverBackgroundService> _logger;
        private readonly IServiceProvider _serviceProvider;

        public ArchiverBackgroundService
            (ILogger<ArchiverBackgroundService> logger, 
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("ArchiverBackgroundService is starting.");

            await ArchiveInactiveJobApplicationsAsync();

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromDays(7), stoppingToken);

                _logger.LogInformation("Weekly scan is running.");
                await ArchiveInactiveJobApplicationsAsync();
            }
        }

        private async Task ArchiveInactiveJobApplicationsAsync()
        {
            _logger.LogInformation("Scanning for inactive job applications to archive...");

            using (var scope = _serviceProvider.CreateScope())
            {
                var repository = scope.ServiceProvider.GetRequiredService<IApplicationRepository>();
                var oneMonthAgo = DateTime.Now.AddMonths(-1);
                var validStatuses = new HashSet<string> { AppStatus_Withdrawn, AppStatus_Rejected, AppStatus_Accepted };
                var archivedCount = await repository.ArchiveApplicationsUpdatedBeforeDateAsync(oneMonthAgo, validStatuses);

                if (archivedCount > 0)
                {
                    _logger.LogInformation("{Count} applications have been archived.", archivedCount);
                }
                else
                {
                    _logger.LogInformation("No applications older than one month found.");
                }
            }
        }
    }
}