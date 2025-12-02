using FriendShare.Core.Configuration;
using FriendShare.Infrastructure.Services;
using Microsoft.Extensions.Options;
using Moq;
using FluentAssertions;

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
    [Trait("Category", "Unit")]
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
        token.Should().NotBeNullOrEmpty();
        token.Split('.').Should().HaveCount(3); // JWT has 3 parts
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void GenerateRefreshToken_ReturnsUniqueTokens()
    {
        // Act
        var token1 = _tokenService.GenerateRefreshToken();
        var token2 = _tokenService.GenerateRefreshToken();

        // Assert
        token1.Should().NotBeNullOrEmpty();
        token2.Should().NotBeNullOrEmpty();
        token1.Should().NotBe(token2);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void GetTokenExpiration_ReturnsFutureDateTime()
    {
        // Act
        var expiration = _tokenService.GetTokenExpiration();

        // Assert
        expiration.Should().BeAfter(DateTime.UtcNow);
        expiration.Should().BeCloseTo(DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes), TimeSpan.FromSeconds(5));
    }

    [Fact]
    [Trait("Category", "Unit")]
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
        principal.Should().NotBeNull();
        principal!.Identity.Should().NotBeNull();
        principal.Claims.Should().NotBeEmpty();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void GetPrincipalFromExpiredToken_WithInvalidToken_ReturnsNull()
    {
        // Arrange
        var invalidToken = "this.is.invalid";

        // Act
        var principal = _tokenService.GetPrincipalFromExpiredToken(invalidToken);

        // Assert
        principal.Should().BeNull();
    }
}
