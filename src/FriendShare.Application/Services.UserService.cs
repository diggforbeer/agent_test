using FriendShare.Core.DTOs;
using FriendShare.Core.Entities;
using FriendShare.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace FriendShare.Application.Services;

/// <summary>
/// Service for handling user profile operations.
/// </summary>
public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<UserService> _logger;

    /// <summary>
    /// Initializes a new instance of the UserService.
    /// </summary>
    public UserService(
        UserManager<ApplicationUser> userManager,
        ILogger<UserService> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<UserDto?> GetUserByIdAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return null;
        }

        return MapToUserDto(user);
    }

    /// <inheritdoc />
    public async Task<UserDto?> UpdateProfileAsync(string userId, UpdateProfileRequest request)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return null;
        }

        user.FirstName = request.FirstName ?? user.FirstName;
        user.LastName = request.LastName ?? user.LastName;
        user.Bio = request.Bio ?? user.Bio;
        user.PhotoUrl = request.PhotoUrl ?? user.PhotoUrl;
        user.UpdatedAt = DateTime.UtcNow;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            _logger.LogWarning("Failed to update profile for user {UserId}", userId);
            return null;
        }

        _logger.LogInformation("User {UserId} updated profile successfully", userId);
        return MapToUserDto(user);
    }

    /// <inheritdoc />
    public async Task<AuthResponse> ChangePasswordAsync(string userId, ChangePasswordRequest request)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return new AuthResponse
            {
                Success = false,
                Message = "User not found."
            };
        }

        var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return new AuthResponse
            {
                Success = false,
                Message = errors
            };
        }

        // Invalidate refresh token to force re-login
        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = null;
        await _userManager.UpdateAsync(user);

        _logger.LogInformation("User {UserId} changed password successfully", userId);

        return new AuthResponse
        {
            Success = true,
            Message = "Password changed successfully. Please log in again."
        };
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAccountAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return false;
        }

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            _logger.LogWarning("Failed to delete account for user {UserId}", userId);
            return false;
        }

        _logger.LogInformation("User {UserId} deleted account successfully", userId);
        return true;
    }

    private static UserDto MapToUserDto(ApplicationUser user)
    {
        return new UserDto
        {
            Id = user.Id,
            UserName = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Bio = user.Bio,
            PhotoUrl = user.PhotoUrl,
            EmailConfirmed = user.EmailConfirmed
        };
    }
}
