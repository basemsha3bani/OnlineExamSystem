using DataRepository;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.IdentityModel.Abstractions;
using ServicesClasseslibrary.Interface.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;

namespace ServicesClasseslibrary.Logging.Implementation
{

    internal class LoggingQueue : ILoggingQueue
    {
        Channel<LogEntry> _channel = Channel.CreateUnbounded<LogEntry>();

        public ChannelReader<LogEntry> Reader => _channel.Reader;

        public void Enqueue(LogEntry entry)
        {
            var writer = _channel.Writer;
            writer.TryWrite(entry);
        }

        
    }
}
