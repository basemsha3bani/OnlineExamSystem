using Domain.DTOs;
using ServicesClasseslibrary.Interface.Analytics;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServicesClasseslibrary.Implmentation.Analytics
{
    public class AnalyticsQueue : IAnalyticsQueue
    {
        private readonly ConcurrentQueue<AnalyticsJobDTO> _queue = new();
        private readonly ConcurrentDictionary<int, byte> _inQueue = new(); // idempotency guard

        public void Enqueue(AnalyticsJobDTO job)
        {
            // prevent duplicate clicks / double enqueue
            if (_inQueue.TryAdd(job.ExaminerId, 0))
            {
                _queue.Enqueue(job);
            }
        }

        public bool TryDequeue(out AnalyticsJobDTO job)
        {
            if (_queue.TryDequeue(out job!))
            {
                _inQueue.TryRemove(job.ExaminerId, out _);
                return true;
            }
            job = null!;
            return false;
        }
    }
   
}
