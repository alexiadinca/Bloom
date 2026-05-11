using Bloom.Api.DTOs.Products;
using Bloom.Api.Models;
using Bloom.Api.Repositories.Interfaces;
using Bloom.Api.Services.Interfaces;

namespace Bloom.Api.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<List<ProductResponse>> GetAllAsync()
    {
        var products = await _productRepository.GetAllAsync();

        return products.Select(MapToResponse).ToList();
    }

    public async Task<ProductResponse?> GetByIdAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);

        if (product is null)
        {
            return null;
        }

        return MapToResponse(product);
    }

    private static ProductResponse MapToResponse(Product product)
    {
        return new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Category = product.Category,
            Price = product.Price,
            ImageUrl = product.ImageUrl,
            StockQuantity = product.StockQuantity
        };
    }
}