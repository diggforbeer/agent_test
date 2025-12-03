using FriendShare.Core.Entities;
using FluentAssertions;

namespace FriendShare.UnitTests;

public class ApplicationUserTests
{
    [Fact]
    [Trait("Category", "Unit")]
    public void ApplicationUser_DefaultConstructor_SetsDefaultValues()
    {
        // Act
        var user = new ApplicationUser();

        // Assert
        user.IsActive.Should().BeTrue();
        user.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        user.UpdatedAt.Should().BeNull();
        user.RefreshToken.Should().BeNull();
        user.RefreshTokenExpiryTime.Should().BeNull();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void ApplicationUser_WithProperties_SetsValuesCorrectly()
    {
        // Arrange
        var userId = "test-user-id";
        var userName = "testuser";
        var email = "test@example.com";
        var firstName = "Test";
        var lastName = "User";
        var bio = "This is a test bio";
        var photoUrl = "https://example.com/photo.jpg";

        // Act
        var user = new ApplicationUser
        {
            Id = userId,
            UserName = userName,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            Bio = bio,
            PhotoUrl = photoUrl,
            EmailConfirmed = true
        };

        // Assert
        user.Id.Should().Be(userId);
        user.UserName.Should().Be(userName);
        user.Email.Should().Be(email);
        user.FirstName.Should().Be(firstName);
        user.LastName.Should().Be(lastName);
        user.Bio.Should().Be(bio);
        user.PhotoUrl.Should().Be(photoUrl);
        user.EmailConfirmed.Should().BeTrue();
        user.IsActive.Should().BeTrue();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void ApplicationUser_CanBeDeactivated()
    {
        // Arrange
        var user = new ApplicationUser
        {
            UserName = "testuser",
            Email = "test@example.com"
        };

        // Act
        user.IsActive = false;

        // Assert
        user.IsActive.Should().BeFalse();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void ApplicationUser_CanStoreRefreshToken()
    {
        // Arrange
        var user = new ApplicationUser
        {
            UserName = "testuser",
            Email = "test@example.com"
        };
        var refreshToken = "test-refresh-token";
        var expiryTime = DateTime.UtcNow.AddDays(7);

        // Act
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = expiryTime;

        // Assert
        user.RefreshToken.Should().Be(refreshToken);
        user.RefreshTokenExpiryTime.Should().Be(expiryTime);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void ApplicationUser_UpdatedAt_CanBeSet()
    {
        // Arrange
        var user = new ApplicationUser
        {
            UserName = "testuser",
            Email = "test@example.com"
        };
        var updateTime = DateTime.UtcNow;

        // Act
        user.UpdatedAt = updateTime;

        // Assert
        user.UpdatedAt.Should().Be(updateTime);
        user.UpdatedAt.Should().BeAfter(user.CreatedAt);
    }
}
