using DataModel;
using ServicesClasseslibrary.Interface.Analytics;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesClasseslibrary.Implmentation.Analytics
{
    public class AnalyticsQueue : IAnalyticsQueue
    {
        private readonly ConcurrentQueue<AnalyticsJob> _queue = new();
        private readonly ConcurrentDictionary<int, byte> _inQueue = new(); // idempotency guard

        public void Enqueue(AnalyticsJob job)
        {
            // prevent duplicate clicks / double enqueue
            if (_inQueue.TryAdd(job.ExaminerId, 0))
            {
                _queue.Enqueue(job);
            }
        }

        public bool TryDequeue(out AnalyticsJob job)
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
