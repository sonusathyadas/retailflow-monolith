using System.Security.Claims;
using System.Web.Http;
using RetailFlow.Application.DTOs;
using RetailFlow.Application.Services;
using RetailFlow.Shared.Models;

namespace RetailFlow.API.Controllers
{
    /// <summary>
    /// Shopping cart operations. Cart data is stored in MongoDB.
    /// </summary>
    [RoutePrefix("api/cart")]
    [Authorize]
    public class CartController : ApiController
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        private int CurrentUserId =>
            int.Parse(((ClaimsPrincipal)User).FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        /// <summary>Get the current user's cart.</summary>
        [HttpGet, Route("")]
        public IHttpActionResult GetCart()
        {
            var cart = _cartService.GetCart(CurrentUserId);
            return Ok(ApiResponse<CartDto>.Ok(cart));
        }

        /// <summary>Add an item to the cart.</summary>
        [HttpPost, Route("items")]
        public IHttpActionResult AddItem([FromBody] AddCartItemRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var cart = _cartService.AddItem(CurrentUserId, request);
            return Ok(ApiResponse<CartDto>.Ok(cart, "Item added to cart."));
        }

        /// <summary>Remove an item from the cart by product ID.</summary>
        [HttpDelete, Route("items/{productId}")]
        public IHttpActionResult RemoveItem(string productId)
        {
            var cart = _cartService.RemoveItem(CurrentUserId, productId);
            return Ok(ApiResponse<CartDto>.Ok(cart, "Item removed."));
        }

        /// <summary>Clear the entire cart.</summary>
        [HttpDelete, Route("")]
        public IHttpActionResult ClearCart()
        {
            _cartService.ClearCart(CurrentUserId);
            return Ok(ApiResponse<object>.Ok(null, "Cart cleared."));
        }
    }
}
