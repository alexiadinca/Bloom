using Bloom.Api.Data;
using Bloom.Api.Models;
using Bloom.Api.Repositories.Interfaces;
using Microsoft.Data.SqlClient;

namespace Bloom.Api.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly SqlConnectionFactory _connectionFactory;

    public OrderRepository(SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<int> CreateOrderAsync(Order order, List<OrderItem> orderItems)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();

        await using var transaction = await connection.BeginTransactionAsync();

        try
        {
            const string insertOrderSql = @"
                INSERT INTO Orders (UserId, ShippingAddress, TotalPrice)
                OUTPUT INSERTED.Id
                VALUES (@UserId, @ShippingAddress, @TotalPrice);
            ";

            await using var insertOrderCommand = new SqlCommand(insertOrderSql, connection);
            insertOrderCommand.Transaction = (SqlTransaction)transaction;

            insertOrderCommand.Parameters.AddWithValue("@UserId", order.UserId);
            insertOrderCommand.Parameters.AddWithValue("@ShippingAddress", order.ShippingAddress);
            insertOrderCommand.Parameters.AddWithValue("@TotalPrice", order.TotalPrice);

            var insertedOrderId = await insertOrderCommand.ExecuteScalarAsync();
            var orderId = Convert.ToInt32(insertedOrderId);

            foreach (var item in orderItems)
            {
                const string insertOrderItemSql = @"
                    INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice, LineTotal)
                    VALUES (@OrderId, @ProductId, @Quantity, @UnitPrice, @LineTotal);
                ";

                await using var insertItemCommand = new SqlCommand(insertOrderItemSql, connection);
                insertItemCommand.Transaction = (SqlTransaction)transaction;

                insertItemCommand.Parameters.AddWithValue("@OrderId", orderId);
                insertItemCommand.Parameters.AddWithValue("@ProductId", item.ProductId);
                insertItemCommand.Parameters.AddWithValue("@Quantity", item.Quantity);
                insertItemCommand.Parameters.AddWithValue("@UnitPrice", item.UnitPrice);
                insertItemCommand.Parameters.AddWithValue("@LineTotal", item.LineTotal);

                await insertItemCommand.ExecuteNonQueryAsync();

                const string updateStockSql = @"
                    UPDATE Products
                    SET StockQuantity = StockQuantity - @Quantity
                    WHERE Id = @ProductId AND StockQuantity >= @Quantity;
                ";

                await using var updateStockCommand = new SqlCommand(updateStockSql, connection);
                updateStockCommand.Transaction = (SqlTransaction)transaction;

                updateStockCommand.Parameters.AddWithValue("@ProductId", item.ProductId);
                updateStockCommand.Parameters.AddWithValue("@Quantity", item.Quantity);

                var affectedRows = await updateStockCommand.ExecuteNonQueryAsync();

                if (affectedRows == 0)
                {
                    throw new InvalidOperationException(
                        $"Insufficient stock for product with id {item.ProductId}."
                    );
                }
            }

            await transaction.CommitAsync();

            return orderId;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}