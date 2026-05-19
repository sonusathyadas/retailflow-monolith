using FluentAssertions;
using Moq;
using RetailFlow.Application.DTOs;
using RetailFlow.Application.Services;
using RetailFlow.Domain.Entities;
using RetailFlow.Domain.Interfaces;
using Xunit;

namespace RetailFlow.UnitTests.Services
{
    public class InventoryServiceTests
    {
        private readonly Mock<IInventoryRepository> _repoMock;
        private readonly InventoryService _sut;

        public InventoryServiceTests()
        {
            _repoMock = new Mock<IInventoryRepository>();
            _sut = new InventoryService(_repoMock.Object);
        }

        [Fact]
        public void GetByProductId_ExistingProduct_ReturnsDto()
        {
            // Arrange
            _repoMock.Setup(r => r.GetByProductId("prod-1"))
                .Returns(new Inventory
                {
                    Id = 1,
                    ProductId = "prod-1",
                    QuantityAvailable = 50,
                    WarehouseLocation = "A1",
                    LowStockThreshold = 10
                });

            // Act
            var result = _sut.GetByProductId("prod-1");

            // Assert
            result.Should().NotBeNull();
            result.QuantityAvailable.Should().Be(50);
            result.IsLowStock.Should().BeFalse();
        }

        [Fact]
        public void GetByProductId_LowStock_SetsIsLowStockTrue()
        {
            // Arrange
            _repoMock.Setup(r => r.GetByProductId("prod-2"))
                .Returns(new Inventory
                {
                    Id = 2,
                    ProductId = "prod-2",
                    QuantityAvailable = 5,
                    LowStockThreshold = 10
                });

            // Act
            var result = _sut.GetByProductId("prod-2");

            // Assert
            result.IsLowStock.Should().BeTrue();
        }

        [Fact]
        public void ReserveStock_Success_ReturnsTrue()
        {
            // Arrange
            _repoMock.Setup(r => r.ReserveStock("prod-1", 5)).Returns(true);

            // Act
            var result = _sut.ReserveStock(new ReserveStockRequest
            {
                ProductId = "prod-1",
                Quantity = 5
            });

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void ReserveStock_InsufficientStock_ReturnsFalse()
        {
            // Arrange
            _repoMock.Setup(r => r.ReserveStock("prod-1", 100)).Returns(false);

            // Act
            var result = _sut.ReserveStock(new ReserveStockRequest
            {
                ProductId = "prod-1",
                Quantity = 100
            });

            // Assert
            result.Should().BeFalse();
        }
    }
}
