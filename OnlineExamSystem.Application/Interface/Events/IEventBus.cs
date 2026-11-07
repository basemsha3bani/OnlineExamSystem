namespace OnlineExamSystem.Application.Interface.Events
{
    public interface IEventBus
    {
        Task PublishAsync<T>(T evt) where T : IEvent;
        Task SubscribeAsync<T>(Func<T, Task> handler) where T : IEvent;
    }
}
