using System.Net;
using System.Web.Http;
using RetailFlow.Application.DTOs;
using RetailFlow.Application.Services;
using RetailFlow.Shared.Models;

namespace RetailFlow.API.Controllers
{
    /// <summary>
    /// Warehouse inventory management.
    /// </summary>
    [RoutePrefix("api/inventory")]
    [Authorize]
    public class InventoryController : ApiController
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        /// <summary>Get inventory level for a product.</summary>
        [HttpGet, Route("{productId}")]
        public IHttpActionResult GetInventory(string productId)
        {
            var inv = _inventoryService.GetByProductId(productId);
            if (inv == null) return NotFound();
            return Ok(ApiResponse<InventoryDto>.Ok(inv));
        }

        /// <summary>Create or update inventory for a product. Requires WarehouseManager or Admin.</summary>
        [HttpPut, Route("")]
        [Authorize(Roles = "Admin,WarehouseManager")]
        public IHttpActionResult UpdateInventory([FromBody] UpdateInventoryRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var inv = _inventoryService.UpdateInventory(request);
            return Ok(ApiResponse<InventoryDto>.Ok(inv, "Inventory updated."));
        }

        /// <summary>Reserve stock for an order.</summary>
        [HttpPost, Route("reserve")]
        [Authorize(Roles = "Admin,WarehouseManager")]
        public IHttpActionResult ReserveStock([FromBody] ReserveStockRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var success = _inventoryService.ReserveStock(request);
            if (!success)
                return Content(HttpStatusCode.Conflict,
                    ApiErrorResponse.Create("INSUFFICIENT_STOCK", "Not enough stock available."));

            return Ok(ApiResponse<object>.Ok(null, "Stock reserved."));
        }
    }
}
