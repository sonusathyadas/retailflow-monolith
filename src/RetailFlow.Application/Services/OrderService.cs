using System;
using System.Collections.Generic;
using System.Linq;
using RetailFlow.Application.DTOs;
using RetailFlow.Domain.Entities;
using RetailFlow.Domain.Enums;
using RetailFlow.Domain.Events;
using RetailFlow.Domain.Interfaces;
using RetailFlow.Messaging.Publishers;
using Serilog;

namespace RetailFlow.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepo;
        private readonly IProductService _productService;
        private readonly IEventPublisher _publisher;
        private static readonly ILogger _log = Log.ForContext<OrderService>();

        public OrderService(IOrderRepository orderRepo, IProductService productService,
            IEventPublisher publisher)
        {
            _orderRepo = orderRepo;
            _productService = productService;
            _publisher = publisher;
        }

        public OrderDto CreateOrder(int userId, CreateOrderRequest request)
        {
            if (request.Items == null || !request.Items.Any())
                throw new ArgumentException("Order must contain at least one item.");

            decimal total = 0;
            var orderItems = new List<OrderItem>();

            foreach (var item in request.Items)
            {
                var product = _productService.GetById(item.ProductId);
                if (product == null)
                    throw new KeyNotFoundException($"Product {item.ProductId} not found.");

                orderItems.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = product.Price
                });

                total += product.Price * item.Quantity;
            }

            var order = new Order
            {
                UserId = userId,
                Status = OrderStatus.Pending,
                TotalAmount = total,
                CreatedAt = DateTime.UtcNow,
                OrderItems = orderItems
            };

            _orderRepo.Add(order);
            _orderRepo.SaveChanges();

            _log.Information("Order {OrderId} created for user {UserId}, total: {Total}",
                order.Id, userId, total);

            // Publish OrderCreated event for async processing
            _publisher.Publish(new OrderCreatedEvent
            {
                OrderId = order.Id.ToString(),
                UserId = userId,
                TotalAmount = total
            });

            return MapToDto(order);
        }

        public OrderDto GetById(int orderId)
        {
            var order = _orderRepo.GetOrderWithItems(orderId);
            return order == null ? null : MapToDto(order);
        }

        public IEnumerable<OrderDto> GetByUserId(int userId)
        {
            return _orderRepo.GetOrdersByUserId(userId).Select(MapToDto);
        }

        public OrderDto CancelOrder(int orderId, int requestingUserId)
        {
            var order = _orderRepo.GetOrderWithItems(orderId);
            if (order == null)
                throw new KeyNotFoundException($"Order {orderId} not found.");

            if (order.UserId != requestingUserId)
                throw new UnauthorizedAccessException("Cannot cancel another user's order.");

            if (order.Status == OrderStatus.Shipped || order.Status == OrderStatus.Delivered)
                throw new InvalidOperationException("Cannot cancel an order that has already shipped.");

            order.Status = OrderStatus.Cancelled;
            order.UpdatedAt = DateTime.UtcNow;
            _orderRepo.Update(order);
            _orderRepo.SaveChanges();

            _log.Information("Order {OrderId} cancelled by user {UserId}", orderId, requestingUserId);
            return MapToDto(order);
        }

        private static OrderDto MapToDto(Order order)
        {
            return new OrderDto
            {
                Id = order.Id,
                UserId = order.UserId,
                Status = order.Status.ToString(),
                TotalAmount = order.TotalAmount,
                CreatedAt = order.CreatedAt,
                Items = order.OrderItems?.Select(i => new OrderItemDto
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    Price = i.Price
                }).ToList()
            };
        }
    }
}
