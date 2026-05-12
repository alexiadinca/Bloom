using Bloom.Api.Data;
using Bloom.Api.Models;
using Bloom.Api.Repositories.Interfaces;
using Microsoft.Data.SqlClient;

namespace Bloom.Api.Repositories;

public class UserRepository : IUserRepository
{
    private readonly SqlConnectionFactory _connectionFactory;

    public UserRepository(SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();

        const string sql = @"
            SELECT Id, FirstName, LastName, Email, PasswordHash, CreatedAt
            FROM Users
            WHERE Email = @Email;
        ";

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Email", email.Trim().ToLower());

        await using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return MapUser(reader);
        }

        return null;
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();

        const string sql = @"
            SELECT Id, FirstName, LastName, Email, PasswordHash, CreatedAt
            FROM Users
            WHERE Id = @Id;
        ";

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", id);

        await using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return MapUser(reader);
        }

        return null;
    }

    public async Task<int> CreateAsync(User user)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();

        const string sql = @"
            INSERT INTO Users (FirstName, LastName, Email, PasswordHash)
            OUTPUT INSERTED.Id
            VALUES (@FirstName, @LastName, @Email, @PasswordHash);
        ";

        await using var command = new SqlCommand(sql, connection);

        command.Parameters.AddWithValue("@FirstName", user.FirstName.Trim());
        command.Parameters.AddWithValue("@LastName", user.LastName.Trim());
        command.Parameters.AddWithValue("@Email", user.Email.Trim().ToLower());
        command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);

        var insertedId = await command.ExecuteScalarAsync();

        return Convert.ToInt32(insertedId);
    }

    private static User MapUser(SqlDataReader reader)
    {
        return new User
        {
            Id = reader.GetInt32(reader.GetOrdinal("Id")),
            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
            LastName = reader.GetString(reader.GetOrdinal("LastName")),
            Email = reader.GetString(reader.GetOrdinal("Email")),
            PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),
            CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
        };
    }
}