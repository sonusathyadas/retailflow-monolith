using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RetailFlow.Application.DTOs
{
    public class AddCartItemRequest
    {
        [Required]
        public string ProductId { get; set; }

        [Range(1, 100)]
        public int Quantity { get; set; }
    }

    public class CartDto
    {
        public int UserId { get; set; }
        public List<CartItemDto> Items { get; set; } = new List<CartItemDto>();
        public decimal TotalPrice { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CartItemDto
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal => Price * Quantity;
    }
}
