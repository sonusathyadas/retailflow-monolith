using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using RetailFlow.Application.DTOs;
using RetailFlow.Application.Services;
using RetailFlow.Shared.Models;

namespace RetailFlow.API.Controllers
{
    /// <summary>
    /// Product catalog management. Products are stored in MongoDB.
    /// </summary>
    [RoutePrefix("api/products")]
    public class ProductsController : ApiController
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>Search and filter products.</summary>
        [HttpGet, Route("")]
        [AllowAnonymous]
        public IHttpActionResult GetProducts([FromUri] ProductSearchRequest request)
        {
            request = request ?? new ProductSearchRequest();
            var products = _productService.Search(request);
            return Ok(ApiResponse<IEnumerable<ProductDto>>.Ok(products));
        }

        /// <summary>Get a single product by ID.</summary>
        [HttpGet, Route("{id}")]
        [AllowAnonymous]
        public IHttpActionResult GetProduct(string id)
        {
            var product = _productService.GetById(id);
            if (product == null) return NotFound();
            return Ok(ApiResponse<ProductDto>.Ok(product));
        }

        /// <summary>Create a new product. Requires Admin role.</summary>
        [HttpPost, Route("")]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult CreateProduct([FromBody] CreateProductRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var product = _productService.Create(request);
            return Content(HttpStatusCode.Created, ApiResponse<ProductDto>.Ok(product, "Product created."));
        }

        /// <summary>Update an existing product. Requires Admin role.</summary>
        [HttpPut, Route("{id}")]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult UpdateProduct(string id, [FromBody] UpdateProductRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var product = _productService.Update(id, request);
                return Ok(ApiResponse<ProductDto>.Ok(product, "Product updated."));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        /// <summary>Soft-delete a product. Requires Admin role.</summary>
        [HttpDelete, Route("{id}")]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult DeleteProduct(string id)
        {
            _productService.Delete(id);
            return Ok(ApiResponse<object>.Ok(null, "Product deleted."));
        }
    }
}
