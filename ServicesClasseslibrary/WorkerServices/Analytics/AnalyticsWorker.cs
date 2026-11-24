using Domain.DTOs;
using Microsoft.Extensions.DependencyInjection;
using ServicesClasseslibrary.WorkerServices.Messaging;
using Microsoft.Extensions.Logging;
using ServicesClasseslibrary.WorkerServices.Interface.Analytics;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServicesClasseslibrary.WorkerServices.Analytics
{
    public class AnalyticsWorker : BackgroundWorkerLoop
    {
        private readonly IAnalyticsQueue _queue;
        private readonly IServiceScopeFactory _scopes;
        private readonly ILogger<AnalyticsWorker> _logger;

        public AnalyticsWorker(IAnalyticsQueue queue, IServiceScopeFactory scopes, ILogger<AnalyticsWorker> logger) : base(logger, TimeSpan.Zero, TimeSpan.FromSeconds(5))
        { _queue = queue; _scopes = scopes; _logger = logger; }

        protected override async Task RunAsync(CancellationToken ct)
        {
            if (_queue.TryDequeue(out AnalyticsJobDTO job))
            {
                using var scope = _scopes.CreateScope();
                var analyticsService = scope.ServiceProvider.GetRequiredService<IAnalyticsService>();
                // A job rebuilds every subject summary for this candidate from all attempts.
                await analyticsService.RecalculateAsync(job);
            }
            else
            {
                await Task.Delay(1000, ct); // idle wait, like logging worker
            }
        }
    }
}
