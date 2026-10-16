using DataRepository;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.IdentityModel.Abstractions;
using Newtonsoft.Json;
using RabbitMQ.Client;
using ServicesClasseslibrary.Interface.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

using System.Threading.Tasks;

namespace ServicesClasseslibrary.Implmentation.Logging
{

    internal class LoggingQueue : ILoggingQueue
    {
        private readonly IConnection _connection;
        
        private readonly IChannel _channel;
        public LoggingQueue(IConnection connection,IChannel channel)
        {
            _channel = channel;
            _connection = connection;   
        }

        public async Task Enqueue(LogEntry entry)
        {
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(entry));
            var props = new BasicProperties { Persistent = true };
            await _channel.BasicPublishAsync("", "exam-logs", false, props, body);
        }
    }
}
