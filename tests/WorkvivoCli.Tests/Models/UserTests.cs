using FluentAssertions;
using Workvivo.Shared.Models;
using Xunit;

namespace WorkvivoCli.Tests.Models;

public class UserTests
{
    [Fact]
    public void User_ShouldHaveCorrectProperties()
    {
        // Arrange & Act
        var user = new User
        {
            Id = 123456,
            ExternalId = "ext-123",
            Email = "test@example.com",
            Name = "Test User",
            FirstName = "Test",
            LastName = "User",
            DisplayName = "Test User",
            JobTitle = "Developer",
            HasLoggedIn = true,
            HasAccess = true
        };

        // Assert
        user.Id.Should().Be(123456);
        user.Email.Should().Be("test@example.com");
        user.Name.Should().Be("Test User");
        user.JobTitle.Should().Be("Developer");
    }

    [Fact]
    public void User_SpaceRole_CanBeSetAndRetrieved()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Email = "test@example.com",
            SpaceRole = "admin"
        };

        // Act & Assert
        user.SpaceRole.Should().Be("admin");
    }
}
