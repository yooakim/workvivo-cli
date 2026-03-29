using FluentAssertions;
using Workvivo.Shared.Configuration;
using Xunit;

namespace WorkvivoCli.Tests.Configuration;

public class WorkvivoSettingsTests
{
    [Fact]
    public void Validate_WithValidSettings_ShouldNotThrow()
    {
        // Arrange
        var settings = new WorkvivoSettings
        {
            ApiToken = "valid-token",
            OrganizationId = "valid-org-id",
            BaseUrl = "https://api.workvivo.com/v1"
        };

        // Act
        var act = () => settings.Validate();

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Validate_WithMissingApiToken_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var settings = new WorkvivoSettings
        {
            ApiToken = "",
            OrganizationId = "valid-org-id",
            BaseUrl = "https://api.workvivo.com/v1"
        };

        // Act
        var act = () => settings.Validate();

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*API Token*");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WithInvalidApiToken_ShouldThrowInvalidOperationException(string? apiToken)
    {
        // Arrange
        var settings = new WorkvivoSettings
        {
            ApiToken = apiToken!,
            OrganizationId = "valid-org-id",
            BaseUrl = "https://api.workvivo.com/v1"
        };

        // Act
        var act = () => settings.Validate();

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }
}
