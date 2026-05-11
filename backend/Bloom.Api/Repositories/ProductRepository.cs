using Bloom.Api.Data;
using Bloom.Api.Models;
using Bloom.Api.Repositories.Interfaces;
using Microsoft.Data.SqlClient;

namespace Bloom.Api.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly SqlConnectionFactory _connectionFactory;

    public ProductRepository(SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<List<Product>> GetAllAsync()
    {
        var products = new List<Product>();

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();

        const string sql = @"
            SELECT Id, Name, Description, Category, Price, ImageUrl, StockQuantity, CreatedAt
            FROM Products
            ORDER BY Id;
        ";

        await using var command = new SqlCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            products.Add(MapProduct(reader));
        }

        return products;
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();

        const string sql = @"
            SELECT Id, Name, Description, Category, Price, ImageUrl, StockQuantity, CreatedAt
            FROM Products
            WHERE Id = @Id;
        ";

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", id);

        await using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return MapProduct(reader);
        }

        return null;
    }

    private static Product MapProduct(SqlDataReader reader)
    {
        return new Product
        {
            Id = reader.GetInt32(reader.GetOrdinal("Id")),
            Name = reader.GetString(reader.GetOrdinal("Name")),
            Description = reader.GetString(reader.GetOrdinal("Description")),
            Category = reader.GetString(reader.GetOrdinal("Category")),
            Price = reader.GetDecimal(reader.GetOrdinal("Price")),
            ImageUrl = reader.GetString(reader.GetOrdinal("ImageUrl")),
            StockQuantity = reader.GetInt32(reader.GetOrdinal("StockQuantity")),
            CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
        };
    }
}