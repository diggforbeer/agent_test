using System.Security.Claims;
using FriendShare.Api.Controllers;
using FriendShare.Core.DTOs;
using FriendShare.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace FriendShare.IntegrationTests;

/// <summary>
/// Integration tests for UserController endpoints.
/// </summary>
public class UserControllerTests
{
    private readonly Mock<IUserService> _mockUserService;
    private readonly Mock<ILogger<UserController>> _mockLogger;
    private const string TestUserId = "test-user-123";
    private const string OtherUserId = "other-user-456";

    public UserControllerTests()
    {
        _mockUserService = new Mock<IUserService>();
        _mockLogger = new Mock<ILogger<UserController>>();
    }

    private UserController CreateController(string userId)
    {
        var controller = new UserController(_mockUserService.Object, _mockLogger.Object);
        
        // Set up authenticated user context
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Name, "testuser"),
            new Claim(ClaimTypes.Email, "test@example.com")
        };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };
        
        return controller;
    }

    [Fact]
    public async Task GetUser_WithValidId_ReturnsUserProfile()
    {
        // Arrange
        var expectedUser = new UserDto
        {
            Id = TestUserId,
            UserName = "testuser",
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User",
            Bio = "Test bio",
            PhotoUrl = "https://example.com/photo.jpg",
            EmailConfirmed = true
        };

        _mockUserService
            .Setup(s => s.GetUserByIdAsync(TestUserId))
            .ReturnsAsync(expectedUser);

        var controller = CreateController(TestUserId);

        // Act
        var result = await controller.GetUser(TestUserId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var user = Assert.IsType<UserDto>(okResult.Value);
        Assert.Equal(expectedUser.Id, user.Id);
        Assert.Equal(expectedUser.Email, user.Email);
    }

    [Fact]
    public async Task GetUser_WithDifferentUserId_ReturnsForbidden()
    {
        // Arrange
        var controller = CreateController(TestUserId);

        // Act
        var result = await controller.GetUser(OtherUserId);

        // Assert
        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task GetUser_WithNonExistentUser_ReturnsNotFound()
    {
        // Arrange
        _mockUserService
            .Setup(s => s.GetUserByIdAsync(TestUserId))
            .ReturnsAsync((UserDto?)null);

        var controller = CreateController(TestUserId);

        // Act
        var result = await controller.GetUser(TestUserId);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task UpdateUser_WithValidRequest_ReturnsUpdatedProfile()
    {
        // Arrange
        var updateRequest = new UpdateProfileRequest
        {
            FirstName = "Updated",
            LastName = "Name",
            Bio = "Updated bio",
            PhotoUrl = "https://example.com/new-photo.jpg"
        };

        var expectedUser = new UserDto
        {
            Id = TestUserId,
            UserName = "testuser",
            Email = "test@example.com",
            FirstName = updateRequest.FirstName,
            LastName = updateRequest.LastName,
            Bio = updateRequest.Bio,
            PhotoUrl = updateRequest.PhotoUrl,
            EmailConfirmed = true
        };

        _mockUserService
            .Setup(s => s.UpdateProfileAsync(TestUserId, It.IsAny<UpdateProfileRequest>()))
            .ReturnsAsync(expectedUser);

        var controller = CreateController(TestUserId);

        // Act
        var result = await controller.UpdateUser(TestUserId, updateRequest);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var user = Assert.IsType<UserDto>(okResult.Value);
        Assert.Equal(expectedUser.FirstName, user.FirstName);
        Assert.Equal(expectedUser.LastName, user.LastName);
    }

    [Fact]
    public async Task UpdateUser_WithDifferentUserId_ReturnsForbidden()
    {
        // Arrange
        var updateRequest = new UpdateProfileRequest
        {
            FirstName = "Updated",
            LastName = "Name"
        };

        var controller = CreateController(TestUserId);

        // Act
        var result = await controller.UpdateUser(OtherUserId, updateRequest);

        // Assert
        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task ChangePassword_WithValidRequest_ReturnsSuccess()
    {
        // Arrange
        var changePasswordRequest = new ChangePasswordRequest
        {
            CurrentPassword = "OldPassword123!",
            NewPassword = "NewPassword123!",
            ConfirmPassword = "NewPassword123!"
        };

        var expectedResponse = new AuthResponse
        {
            Success = true,
            Message = "Password changed successfully. Please log in again."
        };

        _mockUserService
            .Setup(s => s.ChangePasswordAsync(TestUserId, It.IsAny<ChangePasswordRequest>()))
            .ReturnsAsync(expectedResponse);

        var controller = CreateController(TestUserId);

        // Act
        var result = await controller.ChangePassword(TestUserId, changePasswordRequest);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<AuthResponse>(okResult.Value);
        Assert.True(response.Success);
    }

    [Fact]
    public async Task ChangePassword_WithDifferentUserId_ReturnsForbidden()
    {
        // Arrange
        var changePasswordRequest = new ChangePasswordRequest
        {
            CurrentPassword = "OldPassword123!",
            NewPassword = "NewPassword123!",
            ConfirmPassword = "NewPassword123!"
        };

        var controller = CreateController(TestUserId);

        // Act
        var result = await controller.ChangePassword(OtherUserId, changePasswordRequest);

        // Assert
        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task ChangePassword_WithInvalidPassword_ReturnsBadRequest()
    {
        // Arrange
        var changePasswordRequest = new ChangePasswordRequest
        {
            CurrentPassword = "WrongPassword",
            NewPassword = "NewPassword123!",
            ConfirmPassword = "NewPassword123!"
        };

        var expectedResponse = new AuthResponse
        {
            Success = false,
            Message = "Current password is incorrect."
        };

        _mockUserService
            .Setup(s => s.ChangePasswordAsync(TestUserId, It.IsAny<ChangePasswordRequest>()))
            .ReturnsAsync(expectedResponse);

        var controller = CreateController(TestUserId);

        // Act
        var result = await controller.ChangePassword(TestUserId, changePasswordRequest);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task DeleteAccount_WithValidUserId_ReturnsSuccess()
    {
        // Arrange
        _mockUserService
            .Setup(s => s.DeleteAccountAsync(TestUserId))
            .ReturnsAsync(true);

        var controller = CreateController(TestUserId);

        // Act
        var result = await controller.DeleteAccount(TestUserId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task DeleteAccount_WithDifferentUserId_ReturnsForbidden()
    {
        // Arrange
        var controller = CreateController(TestUserId);

        // Act
        var result = await controller.DeleteAccount(OtherUserId);

        // Assert
        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task DeleteAccount_WithNonExistentUser_ReturnsNotFound()
    {
        // Arrange
        _mockUserService
            .Setup(s => s.DeleteAccountAsync(TestUserId))
            .ReturnsAsync(false);

        var controller = CreateController(TestUserId);

        // Act
        var result = await controller.DeleteAccount(TestUserId);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }
}
