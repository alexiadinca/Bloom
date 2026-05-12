using System.ComponentModel.DataAnnotations;

namespace Bloom.Api.DTOs.Orders;

public class CheckoutItemRequest
{
    [Required]
    public int ProductId { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
}