namespace Samples.Cart.Api.Controllers
{
	using System;
	using System.Linq;
	using System.Threading.Tasks;
	using Domain;
	using Microsoft.AspNetCore.Mvc;
	using Models;
	using Timelapse;

	[Route("api/[controller]")]
	[ApiController]
	public class CartController : ControllerBase
	{
		private readonly IRepository<Cart, CartId> _domainRepository;

        public CartController(IRepository<Cart, CartId> domainRepository)
		{
			_domainRepository = domainRepository;
        }

        // GET api/cart/5
        [HttpGet("{id}")]
		public async Task<ActionResult<CartRef>> Get(Guid id)
		{
			var cart = await _domainRepository.GetByIdAsync(new CartId(id)).ConfigureAwait(false);
            return new OkObjectResult(new CartRef
			{
				ProductCount = cart.Products.Sum(x => x.Quantity),
                TotalAmount = cart.Products.Sum(x => x.Quantity * x.UnitPrice)
			});
		}

		[HttpPost]
		public async Task<ActionResult<Guid>> Create()
		{
			var cart = new Cart(new CartId(Guid.NewGuid()));
			await _domainRepository.SaveAsync(cart).ConfigureAwait(false);
			return cart.Id.Id;
		}

		// PUT api/cart/5/addProduct
		[HttpPut("{id}/addProduct")]
		public async Task<ActionResult> AddProduct(Guid id, [FromBody] ProductRef addedProduct)
        {
            var cartId = new CartId(id);
            var cart = await _domainRepository.GetByIdAsync(cartId).ConfigureAwait(false);
			cart.AddProduct(addedProduct);
			await _domainRepository.SaveAsync(cart).ConfigureAwait(false);
            return Ok();
		}

		// PUT api/cart/5/removeProduct
		[HttpPut("{id}/removeProduct")]
		public async Task<ActionResult> RemoveProduct(Guid id, [FromBody] ProductRef removedProduct)
		{
			var cart = await _domainRepository.GetByIdAsync(new CartId(id)).ConfigureAwait(false);
			cart.RemoveProduct(removedProduct);
            await _domainRepository.SaveAsync(cart).ConfigureAwait(false);
            return Ok();
		}
	}
}