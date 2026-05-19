using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RetailFlow.Domain.Enums;

namespace RetailFlow.Application.DTOs
{
    public class CreateOrderRequest
    {
        [Required]
        public List<OrderItemRequest> Items { get; set; }
    }

    public class OrderItemRequest
    {
        [Required]
        public string ProductId { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }

    public class OrderDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<OrderItemDto> Items { get; set; }
    }

    public class OrderItemDto
    {
        public int Id { get; set; }
        public string ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
