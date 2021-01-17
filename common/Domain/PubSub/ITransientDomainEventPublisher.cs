using System.Threading.Tasks;

namespace Domain.PubSub
{
    public interface ITransientDomainEventPublisher
    {
        Task PublishAsync<T>(T publishedEvent);
    }
}