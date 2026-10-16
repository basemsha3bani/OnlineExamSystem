using Microsoft.IdentityModel.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ServicesClasseslibrary.Interface.Logging
{
    public interface ILoggingQueue
    {
        Task Enqueue(LogEntry entry);
       
    }
}
