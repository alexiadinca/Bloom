using System.ComponentModel.DataAnnotations;

namespace Bloom.Api.DTOs.Orders;

public class CheckoutRequest
{
    [Required]
    [MaxLength(500)]
    public string ShippingAddress { get; set; } = string.Empty;

    [Required]
    [MinLength(1)]
    public List<CheckoutItemRequest> Items { get; set; } = new();
}