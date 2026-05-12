using Bloom.Api.Models;

namespace Bloom.Api.Repositories.Interfaces;

public interface IOrderRepository
{
    Task<int> CreateOrderAsync(Order order, List<OrderItem> orderItems);
}