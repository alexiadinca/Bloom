namespace Bloom.Api.DTOs.Orders;

public class OrderResponse
{
    public int OrderId { get; set; }

    public decimal TotalPrice { get; set; }

    public string ShippingAddress { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public List<OrderItemResponse> Items { get; set; } = new();
}