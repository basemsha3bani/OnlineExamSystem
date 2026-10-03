using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Abstractions;
using ServicesClasseslibrary.Interface.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace ServicesClasseslibrary.Logging.Sevices
{
    public class LogginqQueueProcessor
    {
        private ILoggingQueue _loggingQueue;
        ILogger<LogginqQueueProcessor> _logger;
        public LogginqQueueProcessor(ILoggingQueue loggingQueue, ILogger<LogginqQueueProcessor> logger)
        {
            _loggingQueue = loggingQueue;
            _logger = logger;

        }

        public async Task Process(CancellationToken token)
        {
            // FIFO loop - waits when empty, no CPU burn
            await foreach (LogEntry entry in _loggingQueue.Reader.ReadAllAsync(token))
            {
                // This is your actual logging - file, console, anything
                _logger.Log((LogLevel)entry.EventLogLevel, default, entry.Message, null, (state, ex) => state ?? string.Empty);

                // Or file for Dunderly case:
                // await File.AppendAllTextAsync("Logs/log.txt", $"{entry.Time}: {entry.Message}\n");
            }
        }
    }
}
