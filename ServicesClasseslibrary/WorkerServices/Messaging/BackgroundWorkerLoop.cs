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
    public static class BackgroundWorkerLoop
    {
        public static async Task RunAsync(Func<CancellationToken, Task> operation, ILogger logger,
            TimeSpan interval, TimeSpan retryDelay, CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                var delay = interval;
                try { await operation(ct); }
                catch (OperationCanceledException) when (ct.IsCancellationRequested) { break; }
                catch (Exception ex)
                {
                    try { logger.LogError(ex, "Background operation failed; retrying in {RetryDelay} seconds", retryDelay.TotalSeconds); }
                    catch (Exception loggingError)
                    {
                        // A failed Windows Event Log provider must not turn an outage into a host shutdown.
                        Console.Error.WriteLine($"Background operation failed; retrying in {retryDelay.TotalSeconds} seconds. {ex} Logging provider failed: {loggingError.Message}");
                    }
                    delay = retryDelay;
                }
                try { await Task.Delay(delay, ct); }
                catch (OperationCanceledException) when (ct.IsCancellationRequested) { break; }
            }
        }
    }
}
