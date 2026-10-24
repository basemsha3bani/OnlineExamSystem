using RabbitMQ.Client;
using System;
using System.Threading;
using System.Threading.Tasks;
namespace ServicesClasseslibrary.WorkerServices.Messaging
{
    public interface IRabbitMqConnectionProvider
    {
        Task<IChannel> OpenChannelAsync(bool publisherConfirms, CancellationToken ct);
    }
    public sealed class RabbitMqConnectionProvider : IRabbitMqConnectionProvider, IAsyncDisposable
    {
        private readonly IConnectionFactory factory;
        private readonly SemaphoreSlim gate = new(1, 1);
        private IConnection connection;
        public RabbitMqConnectionProvider(IConnectionFactory factory) { this.factory = factory; }
        public async Task<IChannel> OpenChannelAsync(bool publisherConfirms, CancellationToken ct)
        {
            await gate.WaitAsync(ct);
            try
            {
                if (connection == null || !connection.IsOpen)
                {
                    if (connection != null) { await connection.DisposeAsync(); connection = null; }
                    connection = await factory.CreateConnectionAsync(ct);
                }
                return await connection.CreateChannelAsync(new CreateChannelOptions(
                    publisherConfirmationsEnabled: publisherConfirms,
                    publisherConfirmationTrackingEnabled: publisherConfirms), ct);
            }
            finally { gate.Release(); }
        }
        public async ValueTask DisposeAsync()
        {
            if (connection != null) await connection.DisposeAsync();
            gate.Dispose();
        }
    }
}
