using Domain.Core;
using System.Threading.Tasks;

namespace BaseHandler
{
    public interface IDomainEventHandler<TAggregateId, TEvent> where TEvent: IDomainEvent<TAggregateId>
    {
        Task HandleAsync(TEvent @event);
    }
}
