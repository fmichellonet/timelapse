namespace Samples.Cart.Api.Domain.Events
{
	using Timelapse;

	public class CartCreatedEvent : DomainEventBase<CartId>
	{
		private CartCreatedEvent() {}

		internal CartCreatedEvent(CartId cartId) : base(cartId){}

		internal CartCreatedEvent(CartId cartId, long aggregateVersion) : base(cartId, aggregateVersion) {}

		public override IDomainEvent<CartId> WithAggregate(CartId aggregateId, long aggregateVersion)
		{
			return new CartCreatedEvent(aggregateId, aggregateVersion);
		}
	}
}