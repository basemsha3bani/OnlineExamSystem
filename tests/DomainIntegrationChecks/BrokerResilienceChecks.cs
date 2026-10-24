using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using RabbitMQ.Client;
using ServicesClasseslibrary.WorkerServices.ExamEvaluation;
using ServicesClasseslibrary.WorkerServices.Messaging;
using System.Collections.Concurrent;
using System.Reflection;
using System.Net;
using System.Net.Sockets;
static class BrokerResilienceChecks
{
    public static async Task RunAsync()
    {
        var options = new BrokerWorkerOptions { RetryDelay = TimeSpan.FromMilliseconds(25), EvaluationInterval = TimeSpan.FromMilliseconds(25), PublishIdleDelay = TimeSpan.FromMilliseconds(5) };
        var provider = new FakeProvider { FailConnections = 1, FailPublishes = 1 };
        using (var publisher = new RabbitMqPublisher(provider, NullLogger<RabbitMqPublisher>.Instance, options))
        {
            publisher.Enqueue("exam.results.exchange", "result.passed", new byte[] { 1 });
            if (provider.Opens != 0 || publisher.PendingCount != 1) throw new Exception("Enqueue must not connect to the broker");
            await publisher.StartAsync(CancellationToken.None);
            await Until(() => provider.Published == 1 && publisher.PendingCount == 0);
            if (provider.Opens < 3 || publisher.ExecuteTask.IsFaulted) throw new Exception("Failed connects/publishes must be retried without faulting the worker");
            await publisher.StopAsync(CancellationToken.None);
        }
        var consumerProvider = new FakeProvider();
        var consumerService = new RabbitMqConsumer(consumerProvider, NullLogger<RabbitMqConsumer>.Instance);
        using (var cancellation = new CancellationTokenSource())
        {
            var consuming = consumerService.RunAsync(new[] { "one", "two" }, _ => Task.CompletedTask, cancellation.Token);
            await Until(() => consumerProvider.ConsumerRegistrations == 2);
            await Task.Delay(100);
            if (consumerProvider.Opens != 1 || consumerProvider.ConsumerRegistrations != 2 || consuming.IsCompleted)
                throw new Exception("Consumers must be registered once per session");
            cancellation.Cancel();
            try { await consuming; } catch (OperationCanceledException) { }
            if (consumerProvider.Channels.Any(ch => ch.Open)) throw new Exception("Consumer channel must be disposed on shutdown");
        }
        var scopes = new ThrowingScopes();
        using (var worker = new AttemptEvaluationWorker(scopes, NullLogger<AttemptEvaluationWorker>.Instance, options))
        {
            await worker.StartAsync(CancellationToken.None);
            await Until(() => scopes.Calls >= 2);
            if (worker.ExecuteTask.IsFaulted) throw new Exception("Service-resolution failures must not terminate evaluation");
            await worker.StopAsync(CancellationToken.None);
        }
        scopes = new ThrowingScopes();
        using (var checker = new AttemptEvaluationCompletionChecker(scopes, NullLogger<AttemptEvaluationCompletionChecker>.Instance, options))
        {
            await checker.StartAsync(CancellationToken.None);
            await Until(() => scopes.Calls >= 2);
            if (checker.ExecuteTask.IsFaulted) throw new Exception("Service-resolution failures must not terminate notification checking");
            await checker.StopAsync(CancellationToken.None);
        }
        // Real RabbitMQ client against a deliberately closed loopback port: no external broker is changed.
        var listener = new TcpListener(IPAddress.Loopback, 0); listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port; listener.Stop();
        var logger = new CountingLogger<RabbitMqPublisher>();
        await using (var connections = new RabbitMqConnectionProvider(new ConnectionFactory {
            HostName = "127.0.0.1", Port = port, AutomaticRecoveryEnabled = false, RequestedConnectionTimeout = TimeSpan.FromMilliseconds(200) }))
        using (var publisher = new RabbitMqPublisher(connections, logger, options))
        {
            publisher.Enqueue("", "exam-logs", new byte[] { 1 });
            await publisher.StartAsync(CancellationToken.None);
            await Until(() => logger.Errors >= 2, 8000);
            if (publisher.ExecuteTask.IsFaulted || publisher.PendingCount != 1) throw new Exception("An unavailable broker must leave messages pending and the worker alive");
            await publisher.StopAsync(CancellationToken.None);
        }
        var originalError = Console.Error;
        using var capturedError = new StringWriter();
        using var loopCancellation = new CancellationTokenSource();
        int failures = 0;
        try
        {
            Console.SetError(capturedError);
            await BackgroundWorkerLoop.RunAsync(_ => {
                if (++failures < 3) throw new IOException("Simulated outage");
                loopCancellation.Cancel(); return Task.CompletedTask;
            }, new ThrowingLogger(), options.RetryDelay, options.RetryDelay, loopCancellation.Token);
        }
        finally { Console.SetError(originalError); }
        if (failures != 3 || !capturedError.ToString().Contains("Logging provider failed")) throw new Exception("Logging-provider failures must use the fallback and keep retrying");
        Console.WriteLine("Passed broker outage retries, failed-publish retention/recovery, scoped resolution failures, single consumer registration, and cancellation cleanup.");
    }
    static async Task Until(Func<bool> condition, int timeout = 5000)
    {
        using var limit = new CancellationTokenSource(timeout);
        while (!condition()) await Task.Delay(10, limit.Token);
    }
    sealed class ThrowingLogger : ILogger
    {
        public IDisposable BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel level) => true;
        public void Log<TState>(LogLevel level, EventId id, TState state, Exception ex, Func<TState, Exception, string> formatter)
            => throw new InvalidOperationException("Event log denied");
    }
    sealed class ThrowingScopes : IServiceScopeFactory
    {
        public int Calls;
        public IServiceScope CreateScope() { Interlocked.Increment(ref Calls); throw new IOException("Dependency unavailable"); }
    }
    sealed class CountingLogger<T> : ILogger<T>
    {
        public int Errors;
        public IDisposable BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel level) => true;
        public void Log<TState>(LogLevel level, EventId id, TState state, Exception ex, Func<TState, Exception, string> formatter)
        { if (level >= LogLevel.Error) Interlocked.Increment(ref Errors); }
    }
    sealed class FakeProvider : IRabbitMqConnectionProvider
    {
        public int Opens, Published, ConsumerRegistrations, FailConnections, FailPublishes;
        public ConcurrentQueue<TestChannel> Channels = new();
        public Task<IChannel> OpenChannelAsync(bool confirms, CancellationToken ct)
        {
            if (Interlocked.Increment(ref Opens) <= FailConnections) throw new IOException("Connection unavailable");
            var channel = DispatchProxy.Create<IChannel, TestChannel>();
            var proxy = (TestChannel)(object)channel;
            proxy.Publish = () => {
                if (Interlocked.Decrement(ref FailPublishes) >= 0) throw new IOException("Publish failed");
                Interlocked.Increment(ref Published);
            };
            proxy.Consume = () => Interlocked.Increment(ref ConsumerRegistrations);
            Channels.Enqueue(proxy);
            return Task.FromResult(channel);
        }
    }
    public class TestChannel : DispatchProxy
    {
        public bool Open = true;
        public Action Publish, Consume;
        bool topologyDeclared;
        protected override object Invoke(MethodInfo method, object[] args)
        {
            if (method.Name == "get_IsOpen") return Open;
            if (method.Name == "DisposeAsync") { Open = false; return ValueTask.CompletedTask; }
            if (method.Name == "ExchangeDeclareAsync") topologyDeclared = true;
            if (method.Name == "BasicPublishAsync") { if (!topologyDeclared) throw new Exception("Topology must be awaited before publish"); Publish(); }
            if (method.Name == "BasicConsumeAsync") Consume();
            var type = method.ReturnType;
            if (type == typeof(void)) return null;
            if (type == typeof(Task)) return Task.CompletedTask;
            if (type == typeof(ValueTask)) return ValueTask.CompletedTask;
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>))
            {
                var valueType = type.GenericTypeArguments[0];
                var value = valueType.IsValueType ? Activator.CreateInstance(valueType) : null;
                return typeof(Task).GetMethod(nameof(Task.FromResult)).MakeGenericMethod(valueType).Invoke(null, new[] { value });
            }
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }
    }
}
