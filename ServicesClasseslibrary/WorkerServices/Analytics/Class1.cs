using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ServicesClasseslibrary.Interface.Analytics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServicesClasseslibrary.WorkerServices.Analytics
{
    public class AnalyticsWorker : BackgroundService
    {
        private readonly IAnalyticsQueue _queue;
        private readonly IServiceScopeFactory _scopes;
        private readonly ILogger<AnalyticsWorker> _logger;

        public AnalyticsWorker(IAnalyticsQueue queue, IServiceScopeFactory scopes, ILogger<AnalyticsWorker> logger)
        { _queue = queue; _scopes = scopes; _logger = logger; }

        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                if (_queue.TryDequeue(out var job))
                {
                    try
                    {
                        using var scope = _scopes.CreateScope();
                        var analyticsService = scope.ServiceProvider.GetRequiredService<IAnalyticsService>();
                        await analyticsService.RecalculateAsync(job.ExaminerId, job.TriggerAttemptId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Analytics failed for {ExaminerId}", job.ExaminerId);
                        // optionally re-enqueue with delay
                    }
                }
                else
                {
                    await Task.Delay(1000, ct); // idle wait, like logging worker
                }
            }
        }
    }
}
