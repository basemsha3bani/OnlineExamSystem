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
            // Enqueue only buffers locally; observe failures instead of abandoning a Task.
            _loggingQueue.Enqueue(new LogEntry { Message = Message, EventLogLevel = eventLogLevel }).GetAwaiter().GetResult();
        }
    }
}
