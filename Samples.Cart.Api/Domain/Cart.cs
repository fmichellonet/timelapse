namespace Samples.Cart.Api.Domain
{
	using System;
    using System.Collections.Generic;
	using System.Linq;
	using Events;
	using Models;
	using Timelapse.Domain;

	public class Cart : AggregateBase<CartId>
	{
		private readonly Dictionary<string, ProductRef> _products;

		public IEnumerable<ProductRef> Products => _products.Select(x => x.Value);

        private Cart()
        {
            _products = new Dictionary<string, ProductRef>();
        }

        public Cart(CartId cartId) : this()
		{
			RaiseEvent(new CartCreatedEvent(cartId));
		}

		public void Apply(CartCreatedEvent @event)
		{
			Id = @event.AggregateId;
		}

		public void AddProduct(ProductRef addedProduct)
		{
			if (addedProduct == null)
			{
				throw new ArgumentNullException();
			}

			if (addedProduct.Quantity < 1)
			{
				throw new ArgumentOutOfRangeException(nameof(addedProduct.Quantity));
			}

			if (addedProduct.UnitPrice < (decimal) 0.01)
			{
				throw new ArgumentOutOfRangeException(nameof(addedProduct.Quantity));
			}

			RaiseEvent(new ProductAddedEvent(Id, addedProduct));
        }

		public void Apply(ProductAddedEvent @event)
		{
			if (_products.ContainsKey(@event.Product.ProductCode))
			{
				_products[@event.Product.ProductCode].Quantity += @event.Product.Quantity;
			}
			else
			{
                _products.Add(@event.Product.ProductCode, @event.Product);
			}
		}

		public void RemoveProduct(ProductRef removedProduct)
		{
			if (removedProduct == null)
			{
				throw new ArgumentNullException();
			}

			if (removedProduct.Quantity < 1)
			{
				throw new ArgumentOutOfRangeException(nameof(removedProduct.Quantity));
			}

			if (removedProduct.UnitPrice < (decimal)0.01)
			{
				throw new ArgumentOutOfRangeException(nameof(removedProduct.Quantity));
			}

			if (!_products.ContainsKey(removedProduct.ProductCode))
			{
                throw new InvalidOperationException(nameof(removedProduct.ProductCode));
			}

			RaiseEvent(new ProductRemovedEvent(Id, removedProduct));
		}

		public void Apply(ProductRemovedEvent @event)
		{
			_products[@event.Product.ProductCode].Quantity -= @event.Product.Quantity;
        }
    }
}
