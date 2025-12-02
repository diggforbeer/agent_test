using Microsoft.AspNetCore.Identity;

namespace FriendShare.Core.Entities;

/// <summary>
/// Represents a user in the FriendShare application.
/// Extends IdentityUser with additional profile information.
/// </summary>
public class ApplicationUser : IdentityUser
{
    /// <summary>
    /// Gets or sets the user's first name.
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// Gets or sets the user's last name.
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    /// Gets or sets the user's biography.
    /// </summary>
    public string? Bio { get; set; }

    /// <summary>
    /// Gets or sets the URL to the user's profile photo.
    /// </summary>
    public string? PhotoUrl { get; set; }

    /// <summary>
    /// Gets or sets the date when the user was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the date when the user was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets whether the user account is active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the refresh token for JWT authentication.
    /// </summary>
    public string? RefreshToken { get; set; }

    /// <summary>
    /// Gets or sets the expiration time of the refresh token.
    /// </summary>
    public DateTime? RefreshTokenExpiryTime { get; set; }
}
