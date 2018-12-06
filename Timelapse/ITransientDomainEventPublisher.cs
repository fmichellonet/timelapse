namespace Timelapse
{
	using System.Threading.Tasks;

	public interface ITransientDomainEventPublisher
    {
        Task PublishAsync<T>(T publishedEvent);
    }
}