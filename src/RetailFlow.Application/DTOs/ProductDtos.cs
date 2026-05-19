using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RetailFlow.Application.DTOs
{
    public class CreateProductRequest
    {
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public string Category { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();

        public List<string> Images { get; set; } = new List<string>();

        public int InventoryCount { get; set; }
    }

    public class UpdateProductRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public decimal? Price { get; set; }
        public Dictionary<string, string> Attributes { get; set; }
        public List<string> Images { get; set; }
    }

    public class ProductDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public Dictionary<string, string> Attributes { get; set; }
        public List<string> Images { get; set; }
        public int InventoryCount { get; set; }
        public double Rating { get; set; }
    }

    public class ProductSearchRequest
    {
        public string Query { get; set; }
        public string Category { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
