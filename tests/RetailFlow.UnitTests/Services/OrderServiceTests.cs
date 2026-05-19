using System.Collections.Generic;
using FluentAssertions;
using Moq;
using RetailFlow.Application.DTOs;
using RetailFlow.Application.Services;
using RetailFlow.Domain.Entities;
using RetailFlow.Domain.Enums;
using RetailFlow.Domain.Interfaces;
using RetailFlow.Messaging.Publishers;
using Xunit;

namespace RetailFlow.UnitTests.Services
{
    public class OrderServiceTests
    {
        private readonly Mock<IOrderRepository> _orderRepoMock;
        private readonly Mock<IProductService> _productServiceMock;
        private readonly Mock<IEventPublisher> _publisherMock;
        private readonly OrderService _sut;

        public OrderServiceTests()
        {
            _orderRepoMock = new Mock<IOrderRepository>();
            _productServiceMock = new Mock<IProductService>();
            _publisherMock = new Mock<IEventPublisher>();
            _sut = new OrderService(_orderRepoMock.Object, _productServiceMock.Object, _publisherMock.Object);
        }

        [Fact]
        public void CreateOrder_WithValidItems_ReturnsOrderDto()
        {
            // Arrange
            _productServiceMock.Setup(p => p.GetById("prod-1"))
                .Returns(new ProductDto { Id = "prod-1", Name = "Widget", Price = 9.99m });

            var request = new CreateOrderRequest
            {
                Items = new List<OrderItemRequest>
                {
                    new OrderItemRequest { ProductId = "prod-1", Quantity = 2 }
                }
            };

            // Act
            var result = _sut.CreateOrder(userId: 1, request: request);

            // Assert
            result.Should().NotBeNull();
            result.TotalAmount.Should().Be(19.98m);
            result.Status.Should().Be("Pending");
            result.UserId.Should().Be(1);

            _orderRepoMock.Verify(r => r.Add(It.IsAny<Order>()), Times.Once);
            _publisherMock.Verify(p => p.Publish(It.IsAny<RetailFlow.Domain.Events.OrderCreatedEvent>()), Times.Once);
        }

        [Fact]
        public void CreateOrder_WithEmptyItems_ThrowsArgumentException()
        {
            // Arrange
            var request = new CreateOrderRequest { Items = new List<OrderItemRequest>() };

            // Act & Assert
            _sut.Invoking(s => s.CreateOrder(1, request))
                .Should().Throw<System.ArgumentException>();
        }

        [Fact]
        public void CreateOrder_WithUnknownProduct_ThrowsKeyNotFoundException()
        {
            // Arrange
            _productServiceMock.Setup(p => p.GetById("unknown")).Returns((ProductDto)null);

            var request = new CreateOrderRequest
            {
                Items = new List<OrderItemRequest>
                {
                    new OrderItemRequest { ProductId = "unknown", Quantity = 1 }
                }
            };

            // Act & Assert
            _sut.Invoking(s => s.CreateOrder(1, request))
                .Should().Throw<KeyNotFoundException>();
        }

        [Fact]
        public void CancelOrder_ShippedOrder_ThrowsInvalidOperationException()
        {
            // Arrange
            var order = new Order
            {
                Id = 1,
                UserId = 1,
                Status = OrderStatus.Shipped,
                OrderItems = new List<OrderItem>()
            };
            _orderRepoMock.Setup(r => r.GetOrderWithItems(1)).Returns(order);

            // Act & Assert
            _sut.Invoking(s => s.CancelOrder(1, 1))
                .Should().Throw<System.InvalidOperationException>()
                .WithMessage("*shipped*");
        }

        [Fact]
        public void CancelOrder_AnotherUsersOrder_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var order = new Order
            {
                Id = 1,
                UserId = 99,
                Status = OrderStatus.Pending,
                OrderItems = new List<OrderItem>()
            };
            _orderRepoMock.Setup(r => r.GetOrderWithItems(1)).Returns(order);

            // Act & Assert
            _sut.Invoking(s => s.CancelOrder(1, requestingUserId: 1))
                .Should().Throw<System.UnauthorizedAccessException>();
        }
    }
}
