using FriendShare.Application.Services;
using FriendShare.Core.Configuration;
using FriendShare.Core.DTOs;
using FriendShare.Core.Entities;
using FriendShare.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using FluentAssertions;

namespace FriendShare.UnitTests;

public class AuthServiceTests
{
    private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
    private readonly Mock<SignInManager<ApplicationUser>> _mockSignInManager;
    private readonly Mock<ITokenService> _mockTokenService;
    private readonly Mock<IEmailService> _mockEmailService;
    private readonly Mock<ILogger<AuthService>> _mockLogger;
    private readonly AuthService _authService;
    private readonly JwtSettings _jwtSettings;

    public AuthServiceTests()
    {
        var userStore = new Mock<IUserStore<ApplicationUser>>();
        _mockUserManager = new Mock<UserManager<ApplicationUser>>(
            userStore.Object, null, null, null, null, null, null, null, null);

        var contextAccessor = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
        var claimsPrincipalFactory = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
        _mockSignInManager = new Mock<SignInManager<ApplicationUser>>(
            _mockUserManager.Object,
            contextAccessor.Object,
            claimsPrincipalFactory.Object,
            null, null, null, null);

        _mockTokenService = new Mock<ITokenService>();
        _mockEmailService = new Mock<IEmailService>();
        _mockLogger = new Mock<ILogger<AuthService>>();

        _jwtSettings = new JwtSettings
        {
            SecretKey = "TestSecretKeyThatIsAtLeast32CharactersLong!",
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            AccessTokenExpirationMinutes = 15,
            RefreshTokenExpirationDays = 7
        };

        var jwtOptions = Options.Create(_jwtSettings);
        _authService = new AuthService(
            _mockUserManager.Object,
            _mockSignInManager.Object,
            _mockTokenService.Object,
            _mockEmailService.Object,
            jwtOptions,
            _mockLogger.Object);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task RegisterAsync_WithValidData_ReturnsSuccessResponse()
    {
        // Arrange
        var request = new RegisterRequest
        {
            UserName = "testuser",
            Email = "test@example.com",
            Password = "Password123!",
            ConfirmPassword = "Password123!",
            FirstName = "Test",
            LastName = "User"
        };

        _mockUserManager
            .Setup(x => x.FindByEmailAsync(request.Email))
            .ReturnsAsync((ApplicationUser?)null);

        _mockUserManager
            .Setup(x => x.FindByNameAsync(request.UserName))
            .ReturnsAsync((ApplicationUser?)null);

        _mockUserManager
            .Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), request.Password))
            .ReturnsAsync(IdentityResult.Success);

        _mockUserManager
            .Setup(x => x.GenerateEmailConfirmationTokenAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync("test-token");

        _mockEmailService
            .Setup(x => x.SendEmailConfirmationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _authService.RegisterAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Message.Should().Contain("Registration successful");
        _mockEmailService.Verify(x => x.SendEmailConfirmationAsync(request.Email, It.IsAny<string>(), "test-token"), Times.Once);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task RegisterAsync_WithExistingEmail_ReturnsFailureResponse()
    {
        // Arrange
        var request = new RegisterRequest
        {
            UserName = "testuser",
            Email = "existing@example.com",
            Password = "Password123!",
            ConfirmPassword = "Password123!"
        };

        var existingUser = new ApplicationUser { Email = request.Email };
        _mockUserManager
            .Setup(x => x.FindByEmailAsync(request.Email))
            .ReturnsAsync(existingUser);

        // Act
        var result = await _authService.RegisterAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("email already exists");
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task RegisterAsync_WithExistingUsername_ReturnsFailureResponse()
    {
        // Arrange
        var request = new RegisterRequest
        {
            UserName = "existinguser",
            Email = "test@example.com",
            Password = "Password123!",
            ConfirmPassword = "Password123!"
        };

        _mockUserManager
            .Setup(x => x.FindByEmailAsync(request.Email))
            .ReturnsAsync((ApplicationUser?)null);

        var existingUser = new ApplicationUser { UserName = request.UserName };
        _mockUserManager
            .Setup(x => x.FindByNameAsync(request.UserName))
            .ReturnsAsync(existingUser);

        // Act
        var result = await _authService.RegisterAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("username is already taken");
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task LoginAsync_WithValidCredentials_ReturnsSuccessWithToken()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "test@example.com",
            Password = "Password123!"
        };

        var user = new ApplicationUser
        {
            Id = "test-user-id",
            UserName = "testuser",
            Email = request.Email,
            IsActive = true,
            EmailConfirmed = true
        };

        _mockUserManager
            .Setup(x => x.FindByEmailAsync(request.Email))
            .ReturnsAsync(user);

        _mockSignInManager
            .Setup(x => x.CheckPasswordSignInAsync(user, request.Password, true))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

        _mockUserManager
            .Setup(x => x.GetRolesAsync(user))
            .ReturnsAsync(new List<string> { "User" });

        _mockTokenService
            .Setup(x => x.GenerateAccessToken(user.Id, user.Email, user.UserName, It.IsAny<IEnumerable<string>>()))
            .Returns("test-access-token");

        _mockTokenService
            .Setup(x => x.GenerateRefreshToken())
            .Returns("test-refresh-token");

        _mockTokenService
            .Setup(x => x.GetTokenExpiration())
            .Returns(DateTime.UtcNow.AddMinutes(15));

        _mockUserManager
            .Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Token.Should().Be("test-access-token");
        result.RefreshToken.Should().Be("test-refresh-token");
        result.User.Should().NotBeNull();
        result.User!.Email.Should().Be(request.Email);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task LoginAsync_WithNonExistingUser_ReturnsFailureResponse()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "nonexisting@example.com",
            Password = "Password123!"
        };

        _mockUserManager
            .Setup(x => x.FindByEmailAsync(request.Email))
            .ReturnsAsync((ApplicationUser?)null);

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("Invalid email or password");
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task LoginAsync_WithInactiveUser_ReturnsFailureResponse()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "test@example.com",
            Password = "Password123!"
        };

        var user = new ApplicationUser
        {
            Email = request.Email,
            IsActive = false
        };

        _mockUserManager
            .Setup(x => x.FindByEmailAsync(request.Email))
            .ReturnsAsync(user);

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("deactivated");
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task LoginAsync_WithUnconfirmedEmail_ReturnsFailureResponse()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "test@example.com",
            Password = "Password123!"
        };

        var user = new ApplicationUser
        {
            Email = request.Email,
            IsActive = true,
            EmailConfirmed = false
        };

        _mockUserManager
            .Setup(x => x.FindByEmailAsync(request.Email))
            .ReturnsAsync(user);

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("confirm your email");
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task RevokeTokenAsync_WithExistingUser_ReturnsTrue()
    {
        // Arrange
        var userId = "test-user-id";
        var user = new ApplicationUser
        {
            Id = userId,
            RefreshToken = "existing-token",
            RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7)
        };

        _mockUserManager
            .Setup(x => x.FindByIdAsync(userId))
            .ReturnsAsync(user);

        _mockUserManager
            .Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _authService.RevokeTokenAsync(userId);

        // Assert
        result.Should().BeTrue();
        _mockUserManager.Verify(x => x.UpdateAsync(It.Is<ApplicationUser>(u => 
            u.RefreshToken == null && u.RefreshTokenExpiryTime == null)), Times.Once);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task RevokeTokenAsync_WithNonExistingUser_ReturnsFalse()
    {
        // Arrange
        var userId = "non-existing-user";
        _mockUserManager
            .Setup(x => x.FindByIdAsync(userId))
            .ReturnsAsync((ApplicationUser?)null);

        // Act
        var result = await _authService.RevokeTokenAsync(userId);

        // Assert
        result.Should().BeFalse();
    }
}
