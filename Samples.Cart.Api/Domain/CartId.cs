namespace Samples.Cart.Api.Domain
{
	using System;
	using Timelapse;

	public class CartId : IAggregateId
	{
		public Guid Id { get; }
        
		public CartId(Guid id)
		{
			Id = id;
        }

		public string IdAsString()
		{
			return Id.ToString();
		}
	}
}