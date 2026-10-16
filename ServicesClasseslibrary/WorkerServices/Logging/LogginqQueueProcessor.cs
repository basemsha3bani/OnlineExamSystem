using DataRepository.DataRepositoryEntities;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Abstractions;
using ServicesClasseslibrary.Interface.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace ServicesClasseslibrary.WorkerServices.Logging
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

        public void  QueueLogMessage(string Message,EventLogLevel eventLogLevel)
        {
            Task.Run(async () =>
                await _loggingQueue.Enqueue(new LogEntry
                {
                    Message = Message,
                    EventLogLevel = EventLogLevel.Informational,
                })
             );
        }
    }
}
