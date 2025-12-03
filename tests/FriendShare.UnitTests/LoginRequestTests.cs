using System.ComponentModel.DataAnnotations;
using FriendShare.Core.DTOs;
using FluentAssertions;

namespace FriendShare.UnitTests;

public class LoginRequestTests
{
    [Fact]
    [Trait("Category", "Unit")]
    public void LoginRequest_WithValidData_PassesValidation()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "test@example.com",
            Password = "Password123!"
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        validationResults.Should().BeEmpty();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void LoginRequest_WithInvalidEmail_FailsValidation()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "invalid-email",
            Password = "Password123!"
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        validationResults.Should().ContainSingle();
        validationResults.First().MemberNames.Should().Contain("Email");
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void LoginRequest_WithMissingEmail_FailsValidation()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "",
            Password = "Password123!"
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        validationResults.Should().ContainSingle();
        validationResults.First().MemberNames.Should().Contain("Email");
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void LoginRequest_WithMissingPassword_FailsValidation()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "test@example.com",
            Password = ""
        };

        // Act
        var validationResults = ValidateModel(request);

        // Assert
        validationResults.Should().ContainSingle();
        validationResults.First().MemberNames.Should().Contain("Password");
    }

    private static List<ValidationResult> ValidateModel(object model)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, validationContext, validationResults, true);
        return validationResults;
    }
}
