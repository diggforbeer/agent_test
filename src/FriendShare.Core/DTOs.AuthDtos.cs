using System.ComponentModel.DataAnnotations;

namespace FriendShare.Core.DTOs;

/// <summary>
/// DTO for user registration requests.
/// </summary>
public class RegisterRequest
{
    /// <summary>
    /// Gets or sets the username.
    /// </summary>
    [Required]
    [StringLength(50, MinimumLength = 3)]
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the email address.
    /// </summary>
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the password.
    /// </summary>
    [Required]
    [StringLength(100, MinimumLength = 8)]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the password confirmation.
    /// </summary>
    [Required]
    [Compare(nameof(Password))]
    public string ConfirmPassword { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the first name.
    /// </summary>
    [StringLength(50)]
    public string? FirstName { get; set; }

    /// <summary>
    /// Gets or sets the last name.
    /// </summary>
    [StringLength(50)]
    public string? LastName { get; set; }
}

/// <summary>
/// DTO for login requests.
/// </summary>
public class LoginRequest
{
    /// <summary>
    /// Gets or sets the email address.
    /// </summary>
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the password.
    /// </summary>
    [Required]
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// DTO for authentication response containing JWT tokens.
/// </summary>
public class AuthResponse
{
    /// <summary>
    /// Gets or sets whether the authentication was successful.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Gets or sets the message.
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// Gets or sets the JWT access token.
    /// </summary>
    public string? Token { get; set; }

    /// <summary>
    /// Gets or sets the refresh token.
    /// </summary>
    public string? RefreshToken { get; set; }

    /// <summary>
    /// Gets or sets the token expiration time.
    /// </summary>
    public DateTime? Expiration { get; set; }

    /// <summary>
    /// Gets or sets the user information.
    /// </summary>
    public UserDto? User { get; set; }
}

/// <summary>
/// DTO for user profile information.
/// </summary>
public class UserDto
{
    /// <summary>
    /// Gets or sets the user ID.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the username.
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the email address.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the first name.
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// Gets or sets the last name.
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    /// Gets or sets the biography.
    /// </summary>
    public string? Bio { get; set; }

    /// <summary>
    /// Gets or sets the photo URL.
    /// </summary>
    public string? PhotoUrl { get; set; }

    /// <summary>
    /// Gets or sets whether the email is confirmed.
    /// </summary>
    public bool EmailConfirmed { get; set; }
}

/// <summary>
/// DTO for forgot password request.
/// </summary>
public class ForgotPasswordRequest
{
    /// <summary>
    /// Gets or sets the email address.
    /// </summary>
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}

/// <summary>
/// DTO for reset password request.
/// </summary>
public class ResetPasswordRequest
{
    /// <summary>
    /// Gets or sets the email address.
    /// </summary>
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the reset token.
    /// </summary>
    [Required]
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the new password.
    /// </summary>
    [Required]
    [StringLength(100, MinimumLength = 8)]
    public string NewPassword { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the password confirmation.
    /// </summary>
    [Required]
    [Compare(nameof(NewPassword))]
    public string ConfirmPassword { get; set; } = string.Empty;
}

/// <summary>
/// DTO for change password request.
/// </summary>
public class ChangePasswordRequest
{
    /// <summary>
    /// Gets or sets the current password.
    /// </summary>
    [Required]
    public string CurrentPassword { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the new password.
    /// </summary>
    [Required]
    [StringLength(100, MinimumLength = 8)]
    public string NewPassword { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the password confirmation.
    /// </summary>
    [Required]
    [Compare(nameof(NewPassword))]
    public string ConfirmPassword { get; set; } = string.Empty;
}

/// <summary>
/// DTO for updating user profile.
/// </summary>
public class UpdateProfileRequest
{
    /// <summary>
    /// Gets or sets the first name.
    /// </summary>
    [StringLength(50)]
    public string? FirstName { get; set; }

    /// <summary>
    /// Gets or sets the last name.
    /// </summary>
    [StringLength(50)]
    public string? LastName { get; set; }

    /// <summary>
    /// Gets or sets the biography.
    /// </summary>
    [StringLength(500)]
    public string? Bio { get; set; }

    /// <summary>
    /// Gets or sets the photo URL.
    /// </summary>
    [Url]
    [StringLength(500)]
    public string? PhotoUrl { get; set; }
}

/// <summary>
/// DTO for token refresh request.
/// </summary>
public class RefreshTokenRequest
{
    /// <summary>
    /// Gets or sets the expired access token.
    /// </summary>
    [Required]
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the refresh token.
    /// </summary>
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}

/// <summary>
/// DTO for email confirmation request.
/// </summary>
public class ConfirmEmailRequest
{
    /// <summary>
    /// Gets or sets the user ID.
    /// </summary>
    [Required]
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the confirmation token.
    /// </summary>
    [Required]
    public string Token { get; set; } = string.Empty;
}
