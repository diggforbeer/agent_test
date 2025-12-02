using System.ComponentModel.DataAnnotations;
using FriendShare.Core.DTOs;
using FluentAssertions;

namespace FriendShare.UnitTests;

public class RegisterRequestTests
{
    [Fact]
    [Trait("Category", "Unit")]
    public void RegisterRequest_WithValidData_PassesValidation()
    {
        // Arrange
        var request = new RegisterRequest
        {
            UserName = "testuser",
            Email = "test@example.com",
            Password = "Password123!",
            ConfirmPassword = "Password123!",
            FirstName = "Test",
            LastName = "User"
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        validationResults.Should().BeEmpty();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void RegisterRequest_WithShortUsername_FailsValidation()
    {
        // Arrange
        var request = new RegisterRequest
        {
            UserName = "ab", // Too short
            Email = "test@example.com",
            Password = "Password123!",
            ConfirmPassword = "Password123!"
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        validationResults.Should().ContainSingle();
        validationResults.First().MemberNames.Should().Contain("UserName");
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void RegisterRequest_WithInvalidEmail_FailsValidation()
    {
        // Arrange
        var request = new RegisterRequest
        {
            UserName = "testuser",
            Email = "invalid-email", // Invalid email format
            Password = "Password123!",
            ConfirmPassword = "Password123!"
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        validationResults.Should().ContainSingle();
        validationResults.First().MemberNames.Should().Contain("Email");
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void RegisterRequest_WithShortPassword_FailsValidation()
    {
        // Arrange
        var request = new RegisterRequest
        {
            UserName = "testuser",
            Email = "test@example.com",
            Password = "Pass1!", // Too short
            ConfirmPassword = "Pass1!"
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        validationResults.Should().ContainSingle();
        validationResults.First().MemberNames.Should().Contain("Password");
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void RegisterRequest_WithMismatchedPasswords_FailsValidation()
    {
        // Arrange
        var request = new RegisterRequest
        {
            UserName = "testuser",
            Email = "test@example.com",
            Password = "Password123!",
            ConfirmPassword = "DifferentPassword123!"
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        validationResults.Should().ContainSingle();
        validationResults.First().MemberNames.Should().Contain("ConfirmPassword");
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void RegisterRequest_WithMissingRequiredFields_FailsValidation()
    {
        // Arrange
        var request = new RegisterRequest
        {
            UserName = "",
            Email = "",
            Password = "",
            ConfirmPassword = ""
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        validationResults.Should().HaveCountGreaterOrEqualTo(4); // All required fields
    }

    private static List<ValidationResult> ValidateModel(object model)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, validationContext, validationResults, true);
        return validationResults;
    }
}
