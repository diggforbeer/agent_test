using System.Security.Claims;
using FriendShare.Core.DTOs;
using FriendShare.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FriendShare.Api.Controllers;

/// <summary>
/// Controller for user profile operations by user ID.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UserController> _logger;

    /// <summary>
    /// Initializes a new instance of the UserController.
    /// </summary>
    public UserController(IUserService userService, ILogger<UserController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// Gets a user's profile by ID.
    /// </summary>
    /// <param name="id">The user ID.</param>
    /// <returns>The user profile.</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUser(string id)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(currentUserId))
        {
            return Unauthorized();
        }

        // Users can only access their own profile
        if (currentUserId != id)
        {
            _logger.LogWarning("User {CurrentUserId} attempted to access profile of user {TargetUserId}", currentUserId, id);
            return Forbid();
        }

        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
        {
            return NotFound(new { message = "User not found." });
        }

        return Ok(user);
    }

    /// <summary>
    /// Updates a user's profile.
    /// </summary>
    /// <param name="id">The user ID.</param>
    /// <param name="request">The update profile request.</param>
    /// <returns>The updated user profile.</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateProfileRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(currentUserId))
        {
            return Unauthorized();
        }

        // Users can only update their own profile
        if (currentUserId != id)
        {
            _logger.LogWarning("User {CurrentUserId} attempted to update profile of user {TargetUserId}", currentUserId, id);
            return Forbid();
        }

        var user = await _userService.UpdateProfileAsync(id, request);
        if (user == null)
        {
            return NotFound(new { message = "User not found or update failed." });
        }

        return Ok(user);
    }

    /// <summary>
    /// Changes a user's password.
    /// </summary>
    /// <param name="id">The user ID.</param>
    /// <param name="request">The change password request.</param>
    /// <returns>The result of the password change.</returns>
    [HttpPost("{id}/change-password")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ChangePassword(string id, [FromBody] ChangePasswordRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(currentUserId))
        {
            return Unauthorized();
        }

        // Users can only change their own password
        if (currentUserId != id)
        {
            _logger.LogWarning("User {CurrentUserId} attempted to change password of user {TargetUserId}", currentUserId, id);
            return Forbid();
        }

        var result = await _userService.ChangePasswordAsync(id, request);
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Deletes a user's account.
    /// </summary>
    /// <param name="id">The user ID.</param>
    /// <returns>A success response.</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAccount(string id)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(currentUserId))
        {
            return Unauthorized();
        }

        // Users can only delete their own account
        if (currentUserId != id)
        {
            _logger.LogWarning("User {CurrentUserId} attempted to delete account of user {TargetUserId}", currentUserId, id);
            return Forbid();
        }

        var result = await _userService.DeleteAccountAsync(id);
        if (!result)
        {
            return NotFound(new { message = "User not found or deletion failed." });
        }

        return Ok(new { message = "Account deleted successfully." });
    }
}
