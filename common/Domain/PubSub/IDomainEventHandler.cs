using Domain.Core;
using System.Threading.Tasks;

namespace Domain.PubSub
{
    public interface IDomainEventHandler<TAggregateId, TEvent> where TEvent : IDomainEvent<TAggregateId>
    {
        Task HandleAsync(TEvent @event);
    }
}
