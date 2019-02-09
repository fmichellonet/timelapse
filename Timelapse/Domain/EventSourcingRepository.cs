namespace Timelapse.Domain
{
	using System;
	using System.Reflection;
	using System.Threading.Tasks;
	using Persistence;
	using Persistence.Exceptions;
    using Exceptions;

    public class EventSourcingRepository<TAggregate, TAggregateId> :
        IRepository<TAggregate, TAggregateId>
        where TAggregate : AggregateBase<TAggregateId>, IAggregate<TAggregateId>
        where TAggregateId : IAggregateId
    {
        private readonly IEventStore _eventStore;
        private readonly ITransientDomainEventPublisher _publisher;

        public EventSourcingRepository(IEventStore eventStore, ITransientDomainEventPublisher publisher)
        {
			_eventStore = eventStore ?? throw new ArgumentNullException(nameof(eventStore));
            _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
        }

        public async Task<TAggregate> GetByIdAsync(TAggregateId id)
        {
            try
            {
                var aggregate = CreateEmptyAggregate();
                IEventSourcingAggregate<TAggregateId> aggregatePersistence = aggregate;

                foreach (var @event in await _eventStore.ReadEventsAsync(id))
                {
                    aggregatePersistence.ApplyEvent(@event.DomainEvent, @event.EventNumber);
                }
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
                    await _eventStore.AppendEventAsync(@event).ConfigureAwait(false);
                    await _publisher.PublishAsync((dynamic)@event).ConfigureAwait(false);
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
            var constructor = typeof(TAggregate)
                .GetConstructor(
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
                    null, new Type[0], new ParameterModifier[0]);

            if (constructor == null)
            {
                throw new PrivateEmptyConstructorNotFoundException<TAggregate>();
            }

            return (TAggregate)constructor.Invoke(new object[0]);
        }
    }
}