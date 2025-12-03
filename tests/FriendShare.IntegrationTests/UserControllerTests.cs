using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using FriendShare.Core.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using FriendShare.Core.Interfaces;
using Moq;

namespace FriendShare.IntegrationTests;

/// <summary>
/// Integration tests for UserController endpoints.
/// </summary>
public class UserControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly Mock<IUserService> _mockUserService;
    private const string TestUserId = "test-user-123";
    private const string OtherUserId = "other-user-456";

    public UserControllerTests(WebApplicationFactory<Program> factory)
    {
        _mockUserService = new Mock<IUserService>();
        
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Replace IUserService with mock
                services.RemoveAll<IUserService>();
                services.AddScoped<IUserService>(_ => _mockUserService.Object);
            });
        });
    }

    private HttpClient CreateAuthenticatedClient(string userId)
    {
        var client = _factory.CreateClient();
        // In a real scenario, you would generate a proper JWT token here
        // For now, we'll simulate authentication by adding a mock authorization header
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"mock-token-{userId}");
        return client;
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

        var client = CreateAuthenticatedClient(TestUserId);

        // Act
        var response = await client.GetAsync($"/api/user/{TestUserId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var user = await response.Content.ReadFromJsonAsync<UserDto>();
        Assert.NotNull(user);
        Assert.Equal(expectedUser.Id, user.Id);
        Assert.Equal(expectedUser.Email, user.Email);
    }

    [Fact]
    public async Task GetUser_WithDifferentUserId_ReturnsForbidden()
    {
        // Arrange
        var client = CreateAuthenticatedClient(TestUserId);

        // Act
        var response = await client.GetAsync($"/api/user/{OtherUserId}");

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetUser_WithNonExistentUser_ReturnsNotFound()
    {
        // Arrange
        _mockUserService
            .Setup(s => s.GetUserByIdAsync(TestUserId))
            .ReturnsAsync((UserDto?)null);

        var client = CreateAuthenticatedClient(TestUserId);

        // Act
        var response = await client.GetAsync($"/api/user/{TestUserId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
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

        var client = CreateAuthenticatedClient(TestUserId);

        // Act
        var response = await client.PutAsJsonAsync($"/api/user/{TestUserId}", updateRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var user = await response.Content.ReadFromJsonAsync<UserDto>();
        Assert.NotNull(user);
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

        var client = CreateAuthenticatedClient(TestUserId);

        // Act
        var response = await client.PutAsJsonAsync($"/api/user/{OtherUserId}", updateRequest);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
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

        var client = CreateAuthenticatedClient(TestUserId);

        // Act
        var response = await client.PostAsJsonAsync($"/api/user/{TestUserId}/change-password", changePasswordRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(result);
        Assert.True(result.Success);
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

        var client = CreateAuthenticatedClient(TestUserId);

        // Act
        var response = await client.PostAsJsonAsync($"/api/user/{OtherUserId}/change-password", changePasswordRequest);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
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

        var client = CreateAuthenticatedClient(TestUserId);

        // Act
        var response = await client.PostAsJsonAsync($"/api/user/{TestUserId}/change-password", changePasswordRequest);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task DeleteAccount_WithValidUserId_ReturnsSuccess()
    {
        // Arrange
        _mockUserService
            .Setup(s => s.DeleteAccountAsync(TestUserId))
            .ReturnsAsync(true);

        var client = CreateAuthenticatedClient(TestUserId);

        // Act
        var response = await client.DeleteAsync($"/api/user/{TestUserId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(result.TryGetProperty("message", out var message));
        Assert.Equal("Account deleted successfully.", message.GetString());
    }

    [Fact]
    public async Task DeleteAccount_WithDifferentUserId_ReturnsForbidden()
    {
        // Arrange
        var client = CreateAuthenticatedClient(TestUserId);

        // Act
        var response = await client.DeleteAsync($"/api/user/{OtherUserId}");

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task DeleteAccount_WithNonExistentUser_ReturnsNotFound()
    {
        // Arrange
        _mockUserService
            .Setup(s => s.DeleteAccountAsync(TestUserId))
            .ReturnsAsync(false);

        var client = CreateAuthenticatedClient(TestUserId);

        // Act
        var response = await client.DeleteAsync($"/api/user/{TestUserId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
