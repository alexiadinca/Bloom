using Bloom.Api.DTOs.Orders;
using Bloom.Api.Models;
using Bloom.Api.Repositories.Interfaces;
using Bloom.Api.Services.Interfaces;

namespace Bloom.Api.Services;

public class OrderService : IOrderService
{
    private readonly IProductRepository _productRepository;
    private readonly IOrderRepository _orderRepository;

    public OrderService(
        IProductRepository productRepository,
        IOrderRepository orderRepository)
    {
        _productRepository = productRepository;
        _orderRepository = orderRepository;
    }

    public async Task<OrderResponse> CheckoutAsync(int userId, CheckoutRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.ShippingAddress))
        {
            throw new InvalidOperationException("Shipping address is required.");
        }

        if (request.Items.Count == 0)
        {
            throw new InvalidOperationException("Cart cannot be empty.");
        }

        var groupedItems = request.Items
            .GroupBy(item => item.ProductId)
            .Select(group => new CheckoutItemRequest
            {
                ProductId = group.Key,
                Quantity = group.Sum(item => item.Quantity)
            })
            .ToList();

        var orderItems = new List<OrderItem>();

        foreach (var item in groupedItems)
        {
            if (item.Quantity <= 0)
            {
                throw new InvalidOperationException("Quantity must be greater than zero.");
            }

            var product = await _productRepository.GetByIdAsync(item.ProductId);

            if (product is null)
            {
                throw new InvalidOperationException(
                    $"Product with id {item.ProductId} does not exist."
                );
            }

            if (product.StockQuantity < item.Quantity)
            {
                throw new InvalidOperationException(
                    $"Not enough stock for product: {product.Name}."
                );
            }

            var lineTotal = product.Price * item.Quantity;

            orderItems.Add(new OrderItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                Quantity = item.Quantity,
                UnitPrice = product.Price,
                LineTotal = lineTotal
            });
        }

        var totalPrice = orderItems.Sum(item => item.LineTotal);

        var order = new Order
        {
            UserId = userId,
            ShippingAddress = request.ShippingAddress.Trim(),
            TotalPrice = totalPrice
        };

        var orderId = await _orderRepository.CreateOrderAsync(order, orderItems);

        return new OrderResponse
        {
            OrderId = orderId,
            TotalPrice = totalPrice,
            ShippingAddress = order.ShippingAddress,
            CreatedAt = DateTime.UtcNow,
            Items = orderItems.Select(item => new OrderItemResponse
            {
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                LineTotal = item.LineTotal
            }).ToList()
        };
    }
}