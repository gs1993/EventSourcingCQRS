using Domain.Core;
using System.Threading.Tasks;
using Domain.PubSub;
using System.Reflection;
using System;
using Domain.Persistence.EventStore;
using CSharpFunctionalExtensions;

namespace Domain.Persistence
{
    public class EventSourcingRepository<TAggregate, TAggregateId> : IRepository<TAggregate, TAggregateId>
        where TAggregate : AggregateBase<TAggregateId>, IAggregate<TAggregateId>
        where TAggregateId : IAggregateId
    {
        private readonly IEventStore eventStore;
        private readonly ITransientDomainEventPublisher publisher;

        public EventSourcingRepository(IEventStore eventStore, ITransientDomainEventPublisher publisher)
        {
            this.eventStore = eventStore;
            this.publisher = publisher;
        }

        public async Task<Maybe<TAggregate>> GetByIdAsync(TAggregateId id)
        {
            try
            {
                var aggregate = CreateEmptyAggregate();
                IEventSourcingAggregate<TAggregateId> aggregatePersistence = aggregate;

                foreach (var @event in await eventStore.ReadEventsAsync(id))
                    aggregatePersistence.ApplyEvent(@event.DomainEvent, @event.EventNumber);

                if (aggregate == null)
                    return Maybe<TAggregate>.None;

                return aggregate;
            }
            catch (EventStoreAggregateNotFoundException)
            {
                return null;
            }
            catch (EventStoreCommunicationException ex)
            {
                throw new RepositoryException("Unable to access persistence layer", ex);
            }
        }

        public async Task SaveAsync(TAggregate aggregate)
        {
            try
            {
                IEventSourcingAggregate<TAggregateId> aggregatePersistence = aggregate;

                foreach (var @event in aggregatePersistence.GetUncommittedEvents())
                {
                    await eventStore.AppendEventAsync(@event);
                    await publisher.PublishAsync((dynamic)@event);
                }
                aggregatePersistence.ClearUncommittedEvents();
            }
            catch (EventStoreCommunicationException ex)
            {
                throw new RepositoryException("Unable to access persistence layer", ex);
            }
        }

        private TAggregate CreateEmptyAggregate()
        {
            return (TAggregate)typeof(TAggregate)
                    .GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, 
                        null, new Type[0], new ParameterModifier[0])
                    .Invoke(new object[0]);
        }
    }
}
