using Bloom.Api.DTOs.Auth;
using Bloom.Api.Models;
using Bloom.Api.Repositories.Interfaces;
using Bloom.Api.Security;
using Bloom.Api.Services;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Bloom.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly PasswordHasher _passwordHasher;
    private readonly JwtTokenGenerator _jwtTokenGenerator;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _passwordHasher = new PasswordHasher();

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "BloomSuperSecretDevelopmentKey123456789!",
                ["Jwt:Issuer"] = "Bloom.Api",
                ["Jwt:Audience"] = "Bloom.Client",
                ["Jwt:ExpiresInMinutes"] = "60"
            })
            .Build();

        _jwtTokenGenerator = new JwtTokenGenerator(configuration);

        _authService = new AuthService(
            _userRepositoryMock.Object,
            _passwordHasher,
            _jwtTokenGenerator
        );
    }

    [Fact]
    public async Task RegisterAsync_WhenEmailAlreadyExists_ThrowsInvalidOperationException()
    {
        // Arrange
        var request = new RegisterRequest
        {
            FirstName = "Alexia",
            LastName = "Test",
            Email = "alexia@test.com",
            Password = "Password123"
        };

        _userRepositoryMock
            .Setup(repository => repository.GetByEmailAsync("alexia@test.com"))
            .ReturnsAsync(new User
            {
                Id = 1,
                FirstName = "Alexia",
                LastName = "Test",
                Email = "alexia@test.com",
                PasswordHash = "existing-hash"
            });

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _authService.RegisterAsync(request)
        );

        // Assert
        Assert.Equal("A user with this email already exists.", exception.Message);

        _userRepositoryMock.Verify(repository =>
            repository.CreateAsync(It.IsAny<User>()),
            Times.Never
        );
    }

    [Fact]
    public async Task RegisterAsync_WithValidUser_CreatesUserAndReturnsAuthResponse()
    {
        // Arrange
        var request = new RegisterRequest
        {
            FirstName = "Maria",
            LastName = "Demo",
            Email = "Maria@Test.Com",
            Password = "Password123"
        };

        _userRepositoryMock
            .Setup(repository => repository.GetByEmailAsync("maria@test.com"))
            .ReturnsAsync((User?)null);

        _userRepositoryMock
            .Setup(repository => repository.CreateAsync(It.IsAny<User>()))
            .ReturnsAsync(10);

        // Act
        var result = await _authService.RegisterAsync(request);

        // Assert
        Assert.Equal(10, result.UserId);
        Assert.Equal("Maria", result.FirstName);
        Assert.Equal("Demo", result.LastName);
        Assert.Equal("maria@test.com", result.Email);
        Assert.False(string.IsNullOrWhiteSpace(result.Token));

        _userRepositoryMock.Verify(repository =>
            repository.CreateAsync(It.Is<User>(user =>
                user.FirstName == "Maria" &&
                user.LastName == "Demo" &&
                user.Email == "maria@test.com" &&
                user.PasswordHash != request.Password &&
                user.PasswordHash.StartsWith("$2")
            )),
            Times.Once
        );
    }

    [Fact]
    public async Task LoginAsync_WhenUserDoesNotExist_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "missing@test.com",
            Password = "Password123"
        };

        _userRepositoryMock
            .Setup(repository => repository.GetByEmailAsync("missing@test.com"))
            .ReturnsAsync((User?)null);

        // Act
        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _authService.LoginAsync(request)
        );

        // Assert
        Assert.Equal("Invalid email or password.", exception.Message);
    }

    [Fact]
    public async Task LoginAsync_WhenPasswordIsInvalid_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "alexia@test.com",
            Password = "WrongPassword"
        };

        var validPasswordHash = _passwordHasher.HashPassword("Password123");

        _userRepositoryMock
            .Setup(repository => repository.GetByEmailAsync("alexia@test.com"))
            .ReturnsAsync(new User
            {
                Id = 2,
                FirstName = "Alexia",
                LastName = "Test",
                Email = "alexia@test.com",
                PasswordHash = validPasswordHash
            });

        // Act
        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _authService.LoginAsync(request)
        );

        // Assert
        Assert.Equal("Invalid email or password.", exception.Message);
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsAuthResponse()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "alexia@test.com",
            Password = "Password123"
        };

        var passwordHash = _passwordHasher.HashPassword("Password123");

        _userRepositoryMock
            .Setup(repository => repository.GetByEmailAsync("alexia@test.com"))
            .ReturnsAsync(new User
            {
                Id = 2,
                FirstName = "Alexia",
                LastName = "Test",
                Email = "alexia@test.com",
                PasswordHash = passwordHash
            });

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        Assert.Equal(2, result.UserId);
        Assert.Equal("Alexia", result.FirstName);
        Assert.Equal("Test", result.LastName);
        Assert.Equal("alexia@test.com", result.Email);
        Assert.False(string.IsNullOrWhiteSpace(result.Token));
    }
}