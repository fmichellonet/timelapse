namespace Timelapse.Persistence
{
	using System.Collections.Generic;

	public interface IEventSourcingAggregate<TAggregateId>
    {
        long Version { get; }
        void ApplyEvent(IDomainEvent<TAggregateId> @event, long version);
        IEnumerable<IDomainEvent<TAggregateId>> GetUncommittedEvents();
        void ClearUncommittedEvents();
    }
}