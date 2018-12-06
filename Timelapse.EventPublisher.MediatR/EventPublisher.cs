namespace Timelapse.EventPublisher.MediatR
{
	using System.Threading.Tasks;
	using global::MediatR;

	public class EventPublisher : ITransientDomainEventPublisher
	{
		private readonly IMediator _mediator;

		public EventPublisher(IMediator mediator)
		{
			_mediator = mediator;
		}

		public async Task PublishAsync<T>(T publishedEvent)
		{
			var notification = publishedEvent as INotification;
			if (notification == null)
			{
				return;
			}

			await _mediator.Publish(notification).ConfigureAwait(false);
		}
	}
}