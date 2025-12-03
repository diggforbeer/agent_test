using FriendShare.Application.Services;
using FriendShare.Core.DTOs;
using FriendShare.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using FluentAssertions;

namespace FriendShare.UnitTests;

public class UserServiceTests
{
    private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
    private readonly Mock<ILogger<UserService>> _mockLogger;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        var store = new Mock<IUserStore<ApplicationUser>>();
        _mockUserManager = new Mock<UserManager<ApplicationUser>>(
            store.Object, null, null, null, null, null, null, null, null);
        _mockLogger = new Mock<ILogger<UserService>>();
        _userService = new UserService(_mockUserManager.Object, _mockLogger.Object);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GetUserByIdAsync_WithExistingUser_ReturnsUserDto()
    {
        // Arrange
        var userId = "test-user-id";
        var user = new ApplicationUser
        {
            Id = userId,
            UserName = "testuser",
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User",
            Bio = "Test bio",
            PhotoUrl = "https://example.com/photo.jpg",
            EmailConfirmed = true
        };

        _mockUserManager
            .Setup(x => x.FindByIdAsync(userId))
            .ReturnsAsync(user);

        // Act
        var result = await _userService.GetUserByIdAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(userId);
        result.UserName.Should().Be(user.UserName);
        result.Email.Should().Be(user.Email);
        result.FirstName.Should().Be(user.FirstName);
        result.LastName.Should().Be(user.LastName);
        result.Bio.Should().Be(user.Bio);
        result.PhotoUrl.Should().Be(user.PhotoUrl);
        result.EmailConfirmed.Should().BeTrue();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GetUserByIdAsync_WithNonExistingUser_ReturnsNull()
    {
        // Arrange
        var userId = "non-existing-user";
        _mockUserManager
            .Setup(x => x.FindByIdAsync(userId))
            .ReturnsAsync((ApplicationUser?)null);

        // Act
        var result = await _userService.GetUserByIdAsync(userId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task UpdateProfileAsync_WithValidData_ReturnsUpdatedUserDto()
    {
        // Arrange
        var userId = "test-user-id";
        var user = new ApplicationUser
        {
            Id = userId,
            UserName = "testuser",
            Email = "test@example.com",
            FirstName = "Old",
            LastName = "Name"
        };

        var updateRequest = new UpdateProfileRequest
        {
            FirstName = "New",
            LastName = "Name",
            Bio = "Updated bio",
            PhotoUrl = "https://example.com/newphoto.jpg"
        };

        _mockUserManager
            .Setup(x => x.FindByIdAsync(userId))
            .ReturnsAsync(user);

        _mockUserManager
            .Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _userService.UpdateProfileAsync(userId, updateRequest);

        // Assert
        result.Should().NotBeNull();
        result!.FirstName.Should().Be(updateRequest.FirstName);
        result.LastName.Should().Be(updateRequest.LastName);
        result.Bio.Should().Be(updateRequest.Bio);
        result.PhotoUrl.Should().Be(updateRequest.PhotoUrl);
        _mockUserManager.Verify(x => x.UpdateAsync(It.IsAny<ApplicationUser>()), Times.Once);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task UpdateProfileAsync_WithNonExistingUser_ReturnsNull()
    {
        // Arrange
        var userId = "non-existing-user";
        var updateRequest = new UpdateProfileRequest
        {
            FirstName = "New",
            LastName = "Name"
        };

        _mockUserManager
            .Setup(x => x.FindByIdAsync(userId))
            .ReturnsAsync((ApplicationUser?)null);

        // Act
        var result = await _userService.UpdateProfileAsync(userId, updateRequest);

        // Assert
        result.Should().BeNull();
        _mockUserManager.Verify(x => x.UpdateAsync(It.IsAny<ApplicationUser>()), Times.Never);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task UpdateProfileAsync_WhenUpdateFails_ReturnsNull()
    {
        // Arrange
        var userId = "test-user-id";
        var user = new ApplicationUser
        {
            Id = userId,
            UserName = "testuser",
            Email = "test@example.com"
        };

        var updateRequest = new UpdateProfileRequest
        {
            FirstName = "New",
            LastName = "Name"
        };

        _mockUserManager
            .Setup(x => x.FindByIdAsync(userId))
            .ReturnsAsync(user);

        _mockUserManager
            .Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Update failed" }));

        // Act
        var result = await _userService.UpdateProfileAsync(userId, updateRequest);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task ChangePasswordAsync_WithValidData_ReturnsSuccessResponse()
    {
        // Arrange
        var userId = "test-user-id";
        var user = new ApplicationUser
        {
            Id = userId,
            UserName = "testuser",
            Email = "test@example.com"
        };

        var changePasswordRequest = new ChangePasswordRequest
        {
            CurrentPassword = "OldPassword123!",
            NewPassword = "NewPassword123!",
            ConfirmPassword = "NewPassword123!"
        };

        _mockUserManager
            .Setup(x => x.FindByIdAsync(userId))
            .ReturnsAsync(user);

        _mockUserManager
            .Setup(x => x.ChangePasswordAsync(user, changePasswordRequest.CurrentPassword, changePasswordRequest.NewPassword))
            .ReturnsAsync(IdentityResult.Success);

        _mockUserManager
            .Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _userService.ChangePasswordAsync(userId, changePasswordRequest);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Message.Should().Contain("Password changed successfully");
        _mockUserManager.Verify(x => x.ChangePasswordAsync(user, changePasswordRequest.CurrentPassword, changePasswordRequest.NewPassword), Times.Once);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task ChangePasswordAsync_WithNonExistingUser_ReturnsFailureResponse()
    {
        // Arrange
        var userId = "non-existing-user";
        var changePasswordRequest = new ChangePasswordRequest
        {
            CurrentPassword = "OldPassword123!",
            NewPassword = "NewPassword123!",
            ConfirmPassword = "NewPassword123!"
        };

        _mockUserManager
            .Setup(x => x.FindByIdAsync(userId))
            .ReturnsAsync((ApplicationUser?)null);

        // Act
        var result = await _userService.ChangePasswordAsync(userId, changePasswordRequest);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("User not found");
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task ChangePasswordAsync_WithInvalidCurrentPassword_ReturnsFailureResponse()
    {
        // Arrange
        var userId = "test-user-id";
        var user = new ApplicationUser
        {
            Id = userId,
            UserName = "testuser",
            Email = "test@example.com"
        };

        var changePasswordRequest = new ChangePasswordRequest
        {
            CurrentPassword = "WrongPassword123!",
            NewPassword = "NewPassword123!",
            ConfirmPassword = "NewPassword123!"
        };

        _mockUserManager
            .Setup(x => x.FindByIdAsync(userId))
            .ReturnsAsync(user);

        _mockUserManager
            .Setup(x => x.ChangePasswordAsync(user, changePasswordRequest.CurrentPassword, changePasswordRequest.NewPassword))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Current password is incorrect" }));

        // Act
        var result = await _userService.ChangePasswordAsync(userId, changePasswordRequest);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("Current password is incorrect");
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task DeleteAccountAsync_WithExistingUser_ReturnsTrue()
    {
        // Arrange
        var userId = "test-user-id";
        var user = new ApplicationUser
        {
            Id = userId,
            UserName = "testuser",
            Email = "test@example.com"
        };

        _mockUserManager
            .Setup(x => x.FindByIdAsync(userId))
            .ReturnsAsync(user);

        _mockUserManager
            .Setup(x => x.DeleteAsync(user))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _userService.DeleteAccountAsync(userId);

        // Assert
        result.Should().BeTrue();
        _mockUserManager.Verify(x => x.DeleteAsync(user), Times.Once);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task DeleteAccountAsync_WithNonExistingUser_ReturnsFalse()
    {
        // Arrange
        var userId = "non-existing-user";
        _mockUserManager
            .Setup(x => x.FindByIdAsync(userId))
            .ReturnsAsync((ApplicationUser?)null);

        // Act
        var result = await _userService.DeleteAccountAsync(userId);

        // Assert
        result.Should().BeFalse();
        _mockUserManager.Verify(x => x.DeleteAsync(It.IsAny<ApplicationUser>()), Times.Never);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task DeleteAccountAsync_WhenDeleteFails_ReturnsFalse()
    {
        // Arrange
        var userId = "test-user-id";
        var user = new ApplicationUser
        {
            Id = userId,
            UserName = "testuser",
            Email = "test@example.com"
        };

        _mockUserManager
            .Setup(x => x.FindByIdAsync(userId))
            .ReturnsAsync(user);

        _mockUserManager
            .Setup(x => x.DeleteAsync(user))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Delete failed" }));

        // Act
        var result = await _userService.DeleteAccountAsync(userId);

        // Assert
        result.Should().BeFalse();
    }
}
