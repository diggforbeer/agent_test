using FriendShare.Core.Configuration;
using FriendShare.Infrastructure.Services;
using Microsoft.Extensions.Options;
using Moq;

namespace FriendShare.UnitTests;

public class TokenServiceTests
{
    private readonly TokenService _tokenService;
    private readonly JwtSettings _jwtSettings;

    public TokenServiceTests()
    {
        _jwtSettings = new JwtSettings
        {
            SecretKey = "ThisIsATestSecretKeyThatIsAtLeast32CharactersLong!",
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            AccessTokenExpirationMinutes = 15,
            RefreshTokenExpirationDays = 7
        };

        var optionsMock = new Mock<IOptions<JwtSettings>>();
        optionsMock.Setup(o => o.Value).Returns(_jwtSettings);

        _tokenService = new TokenService(optionsMock.Object);
    }

    [Fact]
    public void GenerateAccessToken_ReturnsValidToken()
    {
        // Arrange
        var userId = "test-user-id";
        var email = "test@example.com";
        var userName = "testuser";
        var roles = new List<string> { "User" };

        // Act
        var token = _tokenService.GenerateAccessToken(userId, email, userName, roles);

        // Assert
        Assert.NotNull(token);
        Assert.NotEmpty(token);
    }

    [Fact]
    public void GenerateRefreshToken_ReturnsUniqueTokens()
    {
        // Act
        var token1 = _tokenService.GenerateRefreshToken();
        var token2 = _tokenService.GenerateRefreshToken();

        // Assert
        Assert.NotNull(token1);
        Assert.NotNull(token2);
        Assert.NotEqual(token1, token2);
    }

    [Fact]
    public void GetTokenExpiration_ReturnsFutureDateTime()
    {
        // Act
        var expiration = _tokenService.GetTokenExpiration();

        // Assert
        Assert.True(expiration > DateTime.UtcNow);
    }

    [Fact]
    public void GetPrincipalFromExpiredToken_WithValidToken_ReturnsPrincipal()
    {
        // Arrange
        var userId = "test-user-id";
        var email = "test@example.com";
        var userName = "testuser";
        var roles = new List<string> { "User" };
        var token = _tokenService.GenerateAccessToken(userId, email, userName, roles);

        // Act
        var principal = _tokenService.GetPrincipalFromExpiredToken(token);

        // Assert
        Assert.NotNull(principal);
        Assert.NotNull(principal.Identity);
    }

    [Fact]
    public void GetPrincipalFromExpiredToken_WithInvalidToken_ReturnsNull()
    {
        // Arrange
        var invalidToken = "this.is.invalid";

        // Act
        var principal = _tokenService.GetPrincipalFromExpiredToken(invalidToken);

        // Assert
        Assert.Null(principal);
    }
}
