using Microsoft.IdentityModel.Abstractions;
using Newtonsoft.Json;
using ServicesClasseslibrary.Interface.Logging;
using ServicesClasseslibrary.WorkerServices.Messaging;
using System.Text;
using System.Threading.Tasks;
namespace ServicesClasseslibrary.Implmentation.Logging
{
    internal class LoggingQueue : ILoggingQueue
    {
        private readonly IRabbitMqPublisher publisher;
        public LoggingQueue(IRabbitMqPublisher publisher) { this.publisher = publisher; }
        public Task Enqueue(LogEntry entry)
        {
            publisher.Enqueue("", RabbitMqTopology.LogsQueue, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(entry)));
            return Task.CompletedTask;
        }
    }
}
