using FriendShare.Core.DTOs;

namespace FriendShare.Core.Interfaces;

/// <summary>
/// Interface for authentication service operations.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Registers a new user.
    /// </summary>
    /// <param name="request">The registration request.</param>
    /// <returns>The authentication response.</returns>
    Task<AuthResponse> RegisterAsync(RegisterRequest request);

    /// <summary>
    /// Authenticates a user and returns JWT tokens.
    /// </summary>
    /// <param name="request">The login request.</param>
    /// <returns>The authentication response with tokens.</returns>
    Task<AuthResponse> LoginAsync(LoginRequest request);

    /// <summary>
    /// Confirms a user's email address.
    /// </summary>
    /// <param name="request">The email confirmation request.</param>
    /// <returns>The authentication response.</returns>
    Task<AuthResponse> ConfirmEmailAsync(ConfirmEmailRequest request);

    /// <summary>
    /// Initiates the password reset process.
    /// </summary>
    /// <param name="request">The forgot password request.</param>
    /// <returns>The authentication response.</returns>
    Task<AuthResponse> ForgotPasswordAsync(ForgotPasswordRequest request);

    /// <summary>
    /// Resets the user's password.
    /// </summary>
    /// <param name="request">The reset password request.</param>
    /// <returns>The authentication response.</returns>
    Task<AuthResponse> ResetPasswordAsync(ResetPasswordRequest request);

    /// <summary>
    /// Refreshes the access token using a refresh token.
    /// </summary>
    /// <param name="request">The refresh token request.</param>
    /// <returns>The authentication response with new tokens.</returns>
    Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request);

    /// <summary>
    /// Revokes the user's refresh token (logout).
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <returns>True if successful, false otherwise.</returns>
    Task<bool> RevokeTokenAsync(string userId);
}

/// <summary>
/// Interface for user profile management operations.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Gets a user's profile by ID.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <returns>The user DTO or null if not found.</returns>
    Task<UserDto?> GetUserByIdAsync(string userId);

    /// <summary>
    /// Updates a user's profile.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="request">The update profile request.</param>
    /// <returns>The updated user DTO or null if failed.</returns>
    Task<UserDto?> UpdateProfileAsync(string userId, UpdateProfileRequest request);

    /// <summary>
    /// Changes a user's password.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="request">The change password request.</param>
    /// <returns>True if successful, false otherwise.</returns>
    Task<AuthResponse> ChangePasswordAsync(string userId, ChangePasswordRequest request);

    /// <summary>
    /// Deletes a user's account.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <returns>True if successful, false otherwise.</returns>
    Task<bool> DeleteAccountAsync(string userId);
}

/// <summary>
/// Interface for JWT token service operations.
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Generates a JWT access token for a user.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="email">The user's email.</param>
    /// <param name="userName">The user's name.</param>
    /// <param name="roles">The user's roles.</param>
    /// <returns>The JWT token.</returns>
    string GenerateAccessToken(string userId, string email, string userName, IEnumerable<string> roles);

    /// <summary>
    /// Generates a refresh token.
    /// </summary>
    /// <returns>The refresh token.</returns>
    string GenerateRefreshToken();

    /// <summary>
    /// Gets the principal from an expired token.
    /// </summary>
    /// <param name="token">The expired token.</param>
    /// <returns>The claims principal or null if invalid.</returns>
    System.Security.Claims.ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);

    /// <summary>
    /// Gets the access token expiration time.
    /// </summary>
    DateTime GetTokenExpiration();
}

/// <summary>
/// Interface for email service operations.
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Sends an email confirmation link.
    /// </summary>
    /// <param name="email">The recipient's email address.</param>
    /// <param name="userId">The user ID.</param>
    /// <param name="confirmationToken">The confirmation token.</param>
    /// <returns>A task representing the operation.</returns>
    Task SendEmailConfirmationAsync(string email, string userId, string confirmationToken);

    /// <summary>
    /// Sends a password reset link.
    /// </summary>
    /// <param name="email">The recipient's email address.</param>
    /// <param name="resetToken">The password reset token.</param>
    /// <returns>A task representing the operation.</returns>
    Task SendPasswordResetAsync(string email, string resetToken);
}
