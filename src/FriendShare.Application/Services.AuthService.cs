using FriendShare.Core.Configuration;
using FriendShare.Core.DTOs;
using FriendShare.Core.Entities;
using FriendShare.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FriendShare.Application.Services;

/// <summary>
/// Service for handling authentication operations.
/// </summary>
public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly IEmailService _emailService;
    private readonly JwtSettings _jwtSettings;
    private readonly ILogger<AuthService> _logger;

    /// <summary>
    /// Initializes a new instance of the AuthService.
    /// </summary>
    public AuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ITokenService tokenService,
        IEmailService emailService,
        IOptions<JwtSettings> jwtSettings,
        ILogger<AuthService> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _emailService = emailService;
        _jwtSettings = jwtSettings.Value;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return new AuthResponse
            {
                Success = false,
                Message = "An account with this email already exists."
            };
        }

        var existingUserName = await _userManager.FindByNameAsync(request.UserName);
        if (existingUserName != null)
        {
            return new AuthResponse
            {
                Success = false,
                Message = "This username is already taken."
            };
        }

        var user = new ApplicationUser
        {
            UserName = request.UserName,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            _logger.LogWarning("User registration failed for {Email}: {Errors}", request.Email, errors);
            return new AuthResponse
            {
                Success = false,
                Message = errors
            };
        }

        // Generate email confirmation token
        var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        await _emailService.SendEmailConfirmationAsync(user.Email!, user.Id, confirmationToken);

        _logger.LogInformation("User {UserId} registered successfully", user.Id);

        return new AuthResponse
        {
            Success = true,
            Message = "Registration successful. Please check your email to confirm your account."
        };
    }

    /// <inheritdoc />
    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return new AuthResponse
            {
                Success = false,
                Message = "Invalid email or password."
            };
        }

        if (!user.IsActive)
        {
            return new AuthResponse
            {
                Success = false,
                Message = "Your account has been deactivated."
            };
        }

        if (!user.EmailConfirmed)
        {
            return new AuthResponse
            {
                Success = false,
                Message = "Please confirm your email before logging in."
            };
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);

        if (result.IsLockedOut)
        {
            _logger.LogWarning("User {UserId} is locked out", user.Id);
            return new AuthResponse
            {
                Success = false,
                Message = "Your account is temporarily locked. Please try again later."
            };
        }

        if (!result.Succeeded)
        {
            return new AuthResponse
            {
                Success = false,
                Message = "Invalid email or password."
            };
        }

        var roles = await _userManager.GetRolesAsync(user);
        var token = _tokenService.GenerateAccessToken(user.Id, user.Email!, user.UserName!, roles);
        var refreshToken = _tokenService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays);
        await _userManager.UpdateAsync(user);

        _logger.LogInformation("User {UserId} logged in successfully", user.Id);

        return new AuthResponse
        {
            Success = true,
            Message = "Login successful.",
            Token = token,
            RefreshToken = refreshToken,
            Expiration = _tokenService.GetTokenExpiration(),
            User = MapToUserDto(user)
        };
    }

    /// <inheritdoc />
    public async Task<AuthResponse> ConfirmEmailAsync(ConfirmEmailRequest request)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user == null)
        {
            return new AuthResponse
            {
                Success = false,
                Message = "Invalid confirmation request."
            };
        }

        var result = await _userManager.ConfirmEmailAsync(user, request.Token);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return new AuthResponse
            {
                Success = false,
                Message = $"Email confirmation failed: {errors}"
            };
        }

        _logger.LogInformation("User {UserId} confirmed email successfully", user.Id);

        return new AuthResponse
        {
            Success = true,
            Message = "Email confirmed successfully. You can now log in."
        };
    }

    /// <inheritdoc />
    public async Task<AuthResponse> ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            // Don't reveal that the user doesn't exist
            return new AuthResponse
            {
                Success = true,
                Message = "If an account with that email exists, a password reset link has been sent."
            };
        }

        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
        await _emailService.SendPasswordResetAsync(user.Email!, resetToken);

        _logger.LogInformation("Password reset requested for user {UserId}", user.Id);

        return new AuthResponse
        {
            Success = true,
            Message = "If an account with that email exists, a password reset link has been sent."
        };
    }

    /// <inheritdoc />
    public async Task<AuthResponse> ResetPasswordAsync(ResetPasswordRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return new AuthResponse
            {
                Success = false,
                Message = "Invalid password reset request."
            };
        }

        var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return new AuthResponse
            {
                Success = false,
                Message = $"Password reset failed: {errors}"
            };
        }

        // Invalidate refresh token
        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = null;
        await _userManager.UpdateAsync(user);

        _logger.LogInformation("User {UserId} reset password successfully", user.Id);

        return new AuthResponse
        {
            Success = true,
            Message = "Password reset successful. You can now log in with your new password."
        };
    }

    /// <inheritdoc />
    public async Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request)
    {
        var principal = _tokenService.GetPrincipalFromExpiredToken(request.Token);
        if (principal == null)
        {
            return new AuthResponse
            {
                Success = false,
                Message = "Invalid token."
            };
        }

        var userId = principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return new AuthResponse
            {
                Success = false,
                Message = "Invalid token."
            };
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return new AuthResponse
            {
                Success = false,
                Message = "Invalid or expired refresh token."
            };
        }

        var roles = await _userManager.GetRolesAsync(user);
        var newToken = _tokenService.GenerateAccessToken(user.Id, user.Email!, user.UserName!, roles);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays);
        await _userManager.UpdateAsync(user);

        return new AuthResponse
        {
            Success = true,
            Message = "Token refreshed successfully.",
            Token = newToken,
            RefreshToken = newRefreshToken,
            Expiration = _tokenService.GetTokenExpiration(),
            User = MapToUserDto(user)
        };
    }

    /// <inheritdoc />
    public async Task<bool> RevokeTokenAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return false;
        }

        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = null;
        await _userManager.UpdateAsync(user);

        _logger.LogInformation("User {UserId} logged out successfully", userId);

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
