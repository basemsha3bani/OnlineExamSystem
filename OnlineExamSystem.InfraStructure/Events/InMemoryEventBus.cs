using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineExamSystem.InfraStructure.Events
{
    using OnlineExamSystem.Application.Interface.Events;
    using System.Collections.Concurrent;
    using System.Diagnostics;



    public class InMemoryEventBus : IEventBus
    {
        private readonly ConcurrentDictionary<Type, List<Func<IEvent, Task>>> _handlers = new();
        private static readonly ActivitySource _activitySource = new("ExamSystem");

        public Task SubscribeAsync<T>(Func<T, Task> handler) where T : IEvent
        {
            var list = _handlers.GetOrAdd(typeof(T), _ => new List<Func<IEvent, Task>>());
            lock (list)
            {
                list.Add(e => handler((T)e));
            }
            return Task.CompletedTask;
        }

        public async Task PublishAsync<T>(T evt) where T : IEvent
        {
            // Telemetry: Every publish is a trace linked by CorrelationId
            using var activity = _activitySource.StartActivity($"Publish {typeof(T).Name}");
            activity?.SetTag("event.id", evt.EventId);
            activity?.SetTag("correlation.id", evt.CorrelationId);
            activity?.SetTag("event.type", typeof(T).Name);

            if (!_handlers.TryGetValue(typeof(T), out var list)) return;

            List<Func<IEvent, Task>> snapshot;
            lock (list) snapshot = list.ToList();

            foreach (var handler in snapshot)
            {
                try { await handler(evt); }
                catch (Exception ex)
                {
                    // Never crash publisher because a handler failed
                    Console.WriteLine($"[EventBus] Handler failed for {typeof(T).Name}: {ex.Message}");
                }
            }
        }
    }
}
