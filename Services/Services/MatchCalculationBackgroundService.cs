/*using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;
using System;
using Services.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Services.Services
{
    public class MatchCalculationBackgroundService : BackgroundService
    {
       *//* private readonly MatchCalculationQueue _queue;*//*
        private readonly ILogger<MatchCalculationBackgroundService> _logger;
        private readonly IServiceProvider _serviceProvider;

        public MatchCalculationBackgroundService(MatchCalculationQueue queue, IServiceProvider serviceProvider, ILogger<MatchCalculationBackgroundService> logger)
        {
            _queue = queue;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Match Calculation Background Service is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                var workItem = await _queue.DequeueAsync(stoppingToken);

                try
                {
                    await workItem(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while calculating matches.");
                }
            }

            _logger.LogInformation("Match Calculation Background Service ended.");
        }
    }
}
*/