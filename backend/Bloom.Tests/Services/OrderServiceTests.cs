using Bloom.Api.DTOs.Orders;
using Bloom.Api.Models;
using Bloom.Api.Repositories.Interfaces;
using Bloom.Api.Services;
using Moq;

namespace Bloom.Tests.Services;

public class OrderServiceTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly OrderService _orderService;

    public OrderServiceTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _orderRepositoryMock = new Mock<IOrderRepository>();

        _orderService = new OrderService(
            _productRepositoryMock.Object,
            _orderRepositoryMock.Object
        );
    }

    [Fact]
    public async Task CheckoutAsync_WithValidItems_CalculatesTotalPriceCorrectly()
    {
        // Arrange
        var userId = 1;

        var request = new CheckoutRequest
        {
            ShippingAddress = "Craiova, Strada Exemplu 10",
            Items =
            [
                new CheckoutItemRequest
                {
                    ProductId = 1,
                    Quantity = 2
                },
                new CheckoutItemRequest
                {
                    ProductId = 2,
                    Quantity = 1
                }
            ]
        };

        _productRepositoryMock
            .Setup(repository => repository.GetByIdAsync(1))
            .ReturnsAsync(new Product
            {
                Id = 1,
                Name = "Exam Week Survival Kit",
                Price = 34.99m,
                StockQuantity = 10
            });

        _productRepositoryMock
            .Setup(repository => repository.GetByIdAsync(2))
            .ReturnsAsync(new Product
            {
                Id = 2,
                Name = "Long Study Night Kit",
                Price = 32.99m,
                StockQuantity = 10
            });

        _orderRepositoryMock
            .Setup(repository => repository.CreateOrderAsync(
                It.IsAny<Order>(),
                It.IsAny<List<OrderItem>>()
            ))
            .ReturnsAsync(100);

        // Act
        var result = await _orderService.CheckoutAsync(userId, request);

        // Assert
        Assert.Equal(100, result.OrderId);
        Assert.Equal(102.97m, result.TotalPrice);
        Assert.Equal(2, result.Items.Count);

        Assert.Equal(69.98m, result.Items[0].LineTotal);
        Assert.Equal(32.99m, result.Items[1].LineTotal);

        _orderRepositoryMock.Verify(repository =>
            repository.CreateOrderAsync(
                It.Is<Order>(order =>
                    order.UserId == userId &&
                    order.TotalPrice == 102.97m &&
                    order.ShippingAddress == request.ShippingAddress
                ),
                It.Is<List<OrderItem>>(items =>
                    items.Count == 2 &&
                    items[0].ProductId == 1 &&
                    items[0].Quantity == 2 &&
                    items[0].UnitPrice == 34.99m &&
                    items[0].LineTotal == 69.98m &&
                    items[1].ProductId == 2 &&
                    items[1].Quantity == 1 &&
                    items[1].UnitPrice == 32.99m &&
                    items[1].LineTotal == 32.99m
                )
            ),
            Times.Once
        );
    }

    [Fact]
    public async Task CheckoutAsync_WithDuplicateProductIds_GroupsItemsBeforeCreatingOrder()
    {
        // Arrange
        var userId = 1;

        var request = new CheckoutRequest
        {
            ShippingAddress = "Craiova",
            Items =
            [
                new CheckoutItemRequest
                {
                    ProductId = 1,
                    Quantity = 1
                },
                new CheckoutItemRequest
                {
                    ProductId = 1,
                    Quantity = 2
                }
            ]
        };

        _productRepositoryMock
            .Setup(repository => repository.GetByIdAsync(1))
            .ReturnsAsync(new Product
            {
                Id = 1,
                Name = "Exam Week Survival Kit",
                Price = 34.99m,
                StockQuantity = 10
            });

        _orderRepositoryMock
            .Setup(repository => repository.CreateOrderAsync(
                It.IsAny<Order>(),
                It.IsAny<List<OrderItem>>()
            ))
            .ReturnsAsync(101);

        // Act
        var result = await _orderService.CheckoutAsync(userId, request);

        // Assert
        Assert.Single(result.Items);
        Assert.Equal(3, result.Items[0].Quantity);
        Assert.Equal(104.97m, result.TotalPrice);

        _orderRepositoryMock.Verify(repository =>
            repository.CreateOrderAsync(
                It.IsAny<Order>(),
                It.Is<List<OrderItem>>(items =>
                    items.Count == 1 &&
                    items[0].ProductId == 1 &&
                    items[0].Quantity == 3 &&
                    items[0].LineTotal == 104.97m
                )
            ),
            Times.Once
        );
    }

    [Fact]
    public async Task CheckoutAsync_WithEmptyCart_ThrowsInvalidOperationException()
    {
        // Arrange
        var request = new CheckoutRequest
        {
            ShippingAddress = "Craiova",
            Items = []
        };

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _orderService.CheckoutAsync(1, request)
        );

        // Assert
        Assert.Equal("Cart cannot be empty.", exception.Message);

        _orderRepositoryMock.Verify(repository =>
            repository.CreateOrderAsync(
                It.IsAny<Order>(),
                It.IsAny<List<OrderItem>>()
            ),
            Times.Never
        );
    }

    [Fact]
    public async Task CheckoutAsync_WithMissingProduct_ThrowsInvalidOperationException()
    {
        // Arrange
        var request = new CheckoutRequest
        {
            ShippingAddress = "Craiova",
            Items =
            [
                new CheckoutItemRequest
                {
                    ProductId = 999,
                    Quantity = 1
                }
            ]
        };

        _productRepositoryMock
            .Setup(repository => repository.GetByIdAsync(999))
            .ReturnsAsync((Product?)null);

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _orderService.CheckoutAsync(1, request)
        );

        // Assert
        Assert.Equal("Product with id 999 does not exist.", exception.Message);

        _orderRepositoryMock.Verify(repository =>
            repository.CreateOrderAsync(
                It.IsAny<Order>(),
                It.IsAny<List<OrderItem>>()
            ),
            Times.Never
        );
    }

    [Fact]
    public async Task CheckoutAsync_WithInsufficientStock_ThrowsInvalidOperationException()
    {
        // Arrange
        var request = new CheckoutRequest
        {
            ShippingAddress = "Craiova",
            Items =
            [
                new CheckoutItemRequest
                {
                    ProductId = 1,
                    Quantity = 20
                }
            ]
        };

        _productRepositoryMock
            .Setup(repository => repository.GetByIdAsync(1))
            .ReturnsAsync(new Product
            {
                Id = 1,
                Name = "Exam Week Survival Kit",
                Price = 34.99m,
                StockQuantity = 5
            });

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _orderService.CheckoutAsync(1, request)
        );

        // Assert
        Assert.Equal("Not enough stock for product: Exam Week Survival Kit.", exception.Message);

        _orderRepositoryMock.Verify(repository =>
            repository.CreateOrderAsync(
                It.IsAny<Order>(),
                It.IsAny<List<OrderItem>>()
            ),
            Times.Never
        );
    }
}