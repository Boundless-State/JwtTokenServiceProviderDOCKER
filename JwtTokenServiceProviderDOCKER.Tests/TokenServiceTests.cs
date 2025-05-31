using JwtTokenServiceProviderDOCKER.Services;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace JwtTokenServiceProviderDOCKER.Tests;

public class TokenServiceTests
{
    private readonly TokenService _tokenService;

    public TokenServiceTests()
    {
        var inMemorySettings = new[]
        {
            new KeyValuePair<string, string>("Jwt:Key", "MyTestJwtKeyIsSecureAnd32CharLong"),
            new KeyValuePair<string, string>("Jwt:Issuer", "TestIssuer"),
            new KeyValuePair<string, string>("Jwt:Audience", "TestAudience"),
            new KeyValuePair<string, string>("Jwt:ExpiresInDays", "1")
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        _tokenService = new TokenService(configuration);
    }

    [Fact]
    public void GenerateToken_GenerationOk()
    {
        // Arrange
        var userId = "test-user";
        var role = "Admin";

        // Act
        var token = _tokenService.GenerateToken(userId, role);

        // Assert
        Assert.False(string.IsNullOrEmpty(token));
    }

    [Fact]
    public void ValidateToken_ReturnOKResult()
    {
        // Arrange
        var userId = "test-user";
        var role = "Admin";
        var token = _tokenService.GenerateToken(userId, role);

        // Act
        var result = _tokenService.ValidateToken(token);

        // Assert
        Assert.True(result.ValidToken);
        Assert.Equal(userId, result.UserId);
        Assert.Equal(role, result.Role);
    }
}
