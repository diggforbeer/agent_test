using FriendShare.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace FriendShare.Infrastructure.Services;

/// <summary>
/// Stub email service for development. Replace with actual implementation for production.
/// </summary>
public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;

    /// <summary>
    /// Initializes a new instance of the EmailService.
    /// </summary>
    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public Task SendEmailConfirmationAsync(string email, string userId, string confirmationToken)
    {
        // In production, integrate with an email service (SendGrid, AWS SES, etc.)
        // For development, just log the token
        _logger.LogInformation(
            "Email confirmation for {Email}: UserId={UserId}, Token={Token}",
            email, userId, confirmationToken);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task SendPasswordResetAsync(string email, string resetToken)
    {
        // In production, integrate with an email service (SendGrid, AWS SES, etc.)
        // For development, just log the token
        _logger.LogInformation(
            "Password reset for {Email}: Token={Token}",
            email, resetToken);

        return Task.CompletedTask;
    }
}
