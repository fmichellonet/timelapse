namespace Samples.Cart.Api.Domain.Events
{
	using System;
	using Models;
	using Timelapse;

	public class ProductRemovedEvent : DomainEventBase<CartId>
    {
		public ProductRef Product { get; private set; }

        private ProductRemovedEvent() { }

		internal ProductRemovedEvent(CartId cartId, ProductRef productRef) : base(cartId)
        {
            Product = productRef ?? throw new ArgumentNullException(nameof(productRef));
		}

		internal ProductRemovedEvent(CartId cartId, ProductRef productRef, long aggregateVersion) : base(cartId,
			aggregateVersion)
		{
			Product = productRef ?? throw new ArgumentNullException(nameof(productRef));
        }

        public override IDomainEvent<CartId> WithAggregate(CartId aggregateId, long aggregateVersion)
		{
			return new ProductRemovedEvent(aggregateId, Product, aggregateVersion);
		}
	}
}