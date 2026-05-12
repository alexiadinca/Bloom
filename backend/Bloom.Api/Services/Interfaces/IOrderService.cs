using Bloom.Api.DTOs.Orders;

namespace Bloom.Api.Services.Interfaces;

public interface IOrderService
{
    Task<OrderResponse> CheckoutAsync(int userId, CheckoutRequest request);
}