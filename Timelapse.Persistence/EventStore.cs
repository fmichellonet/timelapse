namespace Timelapse.Persistence
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using Exceptions;
	using Microsoft.WindowsAzure.Storage.Table;
	using Newtonsoft.Json;
	using Streamstone;

	public class EventStore : IEventStore
    {
        private readonly CloudTable _table;

        public EventStore(CloudTable table)
        {
            _table = table;
            table.CreateIfNotExistsAsync();
        }

        public async Task<IEnumerable<Event<TAggregateId>>> ReadEventsAsync<TAggregateId>(TAggregateId id) where TAggregateId : IAggregateId
        {
            var paritionKey = id.IdAsString();
            var partition = new Partition(_table, paritionKey);

            if (! await Stream.ExistsAsync(partition).ConfigureAwait(false))
            {
                throw new EventStoreAggregateNotFoundException($"Aggregate {id.IdAsString()} not found");
            }

            var slice = await Stream.ReadAsync<EventEntity>(partition).ConfigureAwait(false);

			return slice.Events
						.ToList()
						.Select(x =>
							new Event<TAggregateId>(Deserialize<TAggregateId>(x.Type, x.Data), slice.Stream.Version));
		}

        public async Task<AppendResult> AppendEventAsync<TAggregateId>(IDomainEvent<TAggregateId> @event) where TAggregateId : IAggregateId
        {
            var paritionKey = @event.AggregateId.IdAsString();
            var partition = new Partition(_table, paritionKey);

            var openResult = await Stream.TryOpenAsync(partition).ConfigureAwait(false);
            var stream = openResult.Found ? openResult.Stream : new Stream(partition);

            if (stream.Version != @event.AggregateVersion)
            {
                throw new EventStoreConcurrencyException();
            }

			try
			{
				var evtId = EventId.From(@event.EventId.ToString());
				var evtProperties = new
				{
					Id = @event.EventId.ToString(),
					Type = @event.GetType().AssemblyQualifiedName,
					Data = JsonConvert.SerializeObject(@event)
				};

				var writeResult = await Stream.WriteAsync(stream, new EventData(EventId.From(evtId), EventProperties.From(evtProperties)))
											  .ConfigureAwait(false);

				return new AppendResult(writeResult.Events.ToList().Last().Version);
			}
			catch (ConcurrencyConflictException)
			{
				throw new EventStoreConcurrencyException();
			}
		}

        private IDomainEvent<TAggregateId> Deserialize<TAggregateId>(string eventType, string data)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings { ContractResolver = new PrivateSetterContractResolver() };

			return JsonConvert.DeserializeObject(data, Type.GetType(eventType), settings) as IDomainEvent<TAggregateId>;
        }   
    }
}