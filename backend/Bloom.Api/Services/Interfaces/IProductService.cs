using Bloom.Api.DTOs.Products;

namespace Bloom.Api.Services.Interfaces;

public interface IProductService
{
    Task<List<ProductResponse>> GetAllAsync();

    Task<ProductResponse?> GetByIdAsync(int id);
}