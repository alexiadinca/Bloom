using Bloom.Api.Models;

namespace Bloom.Api.Repositories.Interfaces;

public interface IProductRepository
{
    Task<List<Product>> GetAllAsync();

    Task<Product?> GetByIdAsync(int id);
}