using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
namespace ServicesClasseslibrary.WorkerServices.Messaging
{
    public sealed class BrokerWorkerOptions
    {
        public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(5);
        public TimeSpan EvaluationInterval { get; set; } = TimeSpan.FromSeconds(5);
        public TimeSpan PublishIdleDelay { get; set; } = TimeSpan.FromMilliseconds(500);
    }
    public abstract class BackgroundWorkerLoop : BackgroundService
    {
        private readonly ILogger logger;
        private readonly TimeSpan interval;
        private readonly TimeSpan retryDelay;

        protected BackgroundWorkerLoop(ILogger logger, TimeSpan interval, TimeSpan retryDelay)
        {
            this.logger = logger;
            this.interval = interval;
            this.retryDelay = retryDelay;
        }

        protected abstract Task RunAsync(CancellationToken ct);

        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    await RunAsync(ct);
                    await Task.Delay(interval, ct);
                }
                catch (OperationCanceledException) when (ct.IsCancellationRequested) { break; }
                catch (Exception ex)
                {
                    await HandleExceptionAsync(ex, ct);
                }
            }
        }

        protected async Task HandleExceptionAsync(Exception exception, CancellationToken ct)
        {
            try { logger.LogError(exception, "Background operation failed; retrying in {RetryDelay} seconds", retryDelay.TotalSeconds); }
            catch (Exception loggingError)
            {
                // A failed Windows Event Log provider must not turn an outage into a host shutdown.
                Console.Error.WriteLine($"Background operation failed; retrying in {retryDelay.TotalSeconds} seconds. {exception} Logging provider failed: {loggingError.Message}");
            }
            try { await Task.Delay(retryDelay, ct); }
            catch (OperationCanceledException) when (ct.IsCancellationRequested) { }
        }
    }
}