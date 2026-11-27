using FluentAssertions;
using Workvivo.Shared.Models;
using WorkvivoCli.Output;
using Xunit;

namespace WorkvivoCli.Tests.Output;

public class CsvOutputFormatterTests
{
    private readonly CsvOutputFormatter _formatter = new();

    /// <summary>
    /// Captures stdout written by the formatter into a string for assertion.
    /// </summary>
    private string CaptureConsoleOutput(Action action)
    {
        var originalOut = Console.Out;
        try
        {
            using var writer = new StringWriter();
            Console.SetOut(writer);
            action();
            return writer.ToString();
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    // ── FormatUsers ───────────────────────────────────────────────────

    [Fact]
    public void FormatUsers_WithEmptyList_ShouldOutputHeaderOnly()
    {
        // Arrange
        var users = Enumerable.Empty<User>();

        // Act
        var output = CaptureConsoleOutput(() => _formatter.FormatUsers(users));

        // Assert
        var lines = output.TrimEnd().Split(Environment.NewLine);
        lines.Should().HaveCount(1);
        lines[0].Should().Be("id,external_id,email,name,display_name,first_name,last_name,job_title,has_access");
    }

    [Fact]
    public void FormatUsers_WithSingleUser_ShouldOutputHeaderAndOneRow()
    {
        // Arrange
        var users = new List<User>
        {
            new()
            {
                Id = 100,
                ExternalId = "ext-100",
                Email = "alice@example.com",
                Name = "Alice Smith",
                DisplayName = "Alice S.",
                FirstName = "Alice",
                LastName = "Smith",
                JobTitle = "Engineer",
                HasAccess = true
            }
        };

        // Act
        var output = CaptureConsoleOutput(() => _formatter.FormatUsers(users));

        // Assert
        var lines = output.TrimEnd().Split(Environment.NewLine);
        lines.Should().HaveCount(2);
        lines[1].Should().Be("100,ext-100,alice@example.com,Alice Smith,Alice S.,Alice,Smith,Engineer,true");
    }

    [Fact]
    public void FormatUsers_WithMultipleUsers_ShouldOutputHeaderAndMultipleRows()
    {
        // Arrange
        var users = new List<User>
        {
            new() { Id = 1, Email = "one@example.com", Name = "One", DisplayName = "One", FirstName = "O", LastName = "Ne", HasAccess = true },
            new() { Id = 2, Email = "two@example.com", Name = "Two", DisplayName = "Two", FirstName = "T", LastName = "Wo", HasAccess = false },
            new() { Id = 3, Email = "three@example.com", Name = "Three", DisplayName = "Three", FirstName = "Th", LastName = "Ree", HasAccess = true }
        };

        // Act
        var output = CaptureConsoleOutput(() => _formatter.FormatUsers(users));

        // Assert
        var lines = output.TrimEnd().Split(Environment.NewLine);
        lines.Should().HaveCount(4); // 1 header + 3 data rows
        lines[1].Should().Contain("one@example.com");
        lines[2].Should().Contain("two@example.com");
        lines[3].Should().Contain("three@example.com");
    }

    [Fact]
    public void FormatUsers_ShouldNotIncludeAvatarUrl()
    {
        // Arrange
        var users = new List<User>
        {
            new()
            {
                Id = 1,
                Email = "test@example.com",
                Name = "Test",
                DisplayName = "Test",
                FirstName = "T",
                LastName = "Est",
                AvatarUrl = "https://cdn.example.com/avatar.png",
                HasAccess = true
            }
        };

        // Act
        var output = CaptureConsoleOutput(() => _formatter.FormatUsers(users));

        // Assert
        output.Should().NotContain("avatar");
        output.Should().NotContain("https://cdn.example.com/avatar.png");
    }

    [Fact]
    public void FormatUsers_WithNullOptionalFields_ShouldOutputEmptyValues()
    {
        // Arrange
        var users = new List<User>
        {
            new()
            {
                Id = 42,
                ExternalId = null,
                Email = "test@example.com",
                Name = "Test",
                DisplayName = "Test",
                FirstName = "T",
                LastName = "Est",
                JobTitle = null,
                HasAccess = false
            }
        };

        // Act
        var output = CaptureConsoleOutput(() => _formatter.FormatUsers(users));

        // Assert
        var lines = output.TrimEnd().Split(Environment.NewLine);
        lines.Should().HaveCount(2);
        // ExternalId and JobTitle should be empty (between commas)
        lines[1].Should().Be("42,,test@example.com,Test,Test,T,Est,,false");
    }

    // ── FormatUser (single/detail) ────────────────────────────────────

    [Fact]
    public void FormatUser_ShouldOutputDetailHeaderAndRow()
    {
        // Arrange
        var user = new User
        {
            Id = 200,
            ExternalId = "ext-200",
            Email = "bob@example.com",
            Name = "Bob Jones",
            DisplayName = "Bob J.",
            FirstName = "Bob",
            LastName = "Jones",
            JobTitle = "Manager",
            Timezone = "Europe/London",
            Locale = "en-GB",
            HireDate = new DateTime(2023, 6, 15),
            ManagerId = 100,
            HasLoggedIn = true,
            IsFrontline = false,
            HasAccess = true,
            Permalink = "https://workvivo.com/user/200"
        };

        // Act
        var output = CaptureConsoleOutput(() => _formatter.FormatUser(user));

        // Assert
        var lines = output.TrimEnd().Split(Environment.NewLine);
        lines.Should().HaveCount(2);
        lines[0].Should().Contain("timezone");
        lines[0].Should().Contain("locale");
        lines[0].Should().Contain("hire_date");
        lines[0].Should().Contain("manager_id");
        lines[0].Should().Contain("has_logged_in");
        lines[0].Should().Contain("is_frontline");
        lines[0].Should().Contain("permalink");
        lines[1].Should().Contain("Europe/London");
        lines[1].Should().Contain("en-GB");
        lines[1].Should().Contain("2023-06-15");
        lines[1].Should().Contain("100");
        lines[1].Should().Contain("https://workvivo.com/user/200");
    }

    [Fact]
    public void FormatUser_ShouldNotIncludeAvatarUrl()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Email = "test@example.com",
            Name = "Test",
            DisplayName = "Test",
            FirstName = "T",
            LastName = "Est",
            AvatarUrl = "https://cdn.example.com/big-avatar.png",
            HasAccess = true
        };

        // Act
        var output = CaptureConsoleOutput(() => _formatter.FormatUser(user));

        // Assert
        output.Should().NotContain("avatar");
        output.Should().NotContain("https://cdn.example.com/big-avatar.png");
    }

    [Fact]
    public void FormatUser_WithNullOptionalFields_ShouldOutputEmptyValues()
    {
        // Arrange
        var user = new User
        {
            Id = 10,
            Email = "min@example.com",
            Name = "Min",
            DisplayName = "Min",
            FirstName = "M",
            LastName = "In",
            ExternalId = null,
            JobTitle = null,
            Timezone = null,
            Locale = null,
            HireDate = null,
            ManagerId = null,
            Permalink = null,
            HasLoggedIn = false,
            IsFrontline = false,
            HasAccess = false
        };

        // Act
        var output = CaptureConsoleOutput(() => _formatter.FormatUser(user));

        // Assert
        var lines = output.TrimEnd().Split(Environment.NewLine);
        lines.Should().HaveCount(2);
        // Nullable fields should result in empty values (consecutive commas)
        lines[1].Should().Contain(",,"); // at least some empty fields in sequence
        lines[1].Should().EndWith("false,false,false,");
    }

    // ── FormatSpaces ──────────────────────────────────────────────────

    [Fact]
    public void FormatSpaces_WithEmptyList_ShouldOutputHeaderOnly()
    {
        // Arrange
        var spaces = Enumerable.Empty<Space>();

        // Act
        var output = CaptureConsoleOutput(() => _formatter.FormatSpaces(spaces));

        // Assert
        var lines = output.TrimEnd().Split(Environment.NewLine);
        lines.Should().HaveCount(1);
        lines[0].Should().Be("id,name,visibility,is_corporate,is_external,categories");
    }

    [Fact]
    public void FormatSpaces_WithSingleSpace_ShouldOutputHeaderAndOneRow()
    {
        // Arrange
        var spaces = new List<Space>
        {
            new()
            {
                Id = 500,
                Name = "Engineering",
                Visibility = "public",
                IsCorporate = true,
                IsExternal = false,
                Categories = new List<Category>
                {
                    new() { Id = 1, Name = "Tech" }
                }
            }
        };

        // Act
        var output = CaptureConsoleOutput(() => _formatter.FormatSpaces(spaces));

        // Assert
        var lines = output.TrimEnd().Split(Environment.NewLine);
        lines.Should().HaveCount(2);
        lines[1].Should().Be("500,Engineering,public,true,false,Tech");
    }

    [Fact]
    public void FormatSpaces_WithMultipleCategories_ShouldJoinWithSemicolon()
    {
        // Arrange
        var spaces = new List<Space>
        {
            new()
            {
                Id = 1,
                Name = "General",
                Visibility = "private",
                IsCorporate = false,
                IsExternal = false,
                Categories = new List<Category>
                {
                    new() { Id = 1, Name = "HR" },
                    new() { Id = 2, Name = "Finance" },
                    new() { Id = 3, Name = "Ops" }
                }
            }
        };

        // Act
        var output = CaptureConsoleOutput(() => _formatter.FormatSpaces(spaces));

        // Assert
        var lines = output.TrimEnd().Split(Environment.NewLine);
        lines.Should().HaveCount(2);
        // Categories with semicolons don't contain commas, so no quoting needed
        lines[1].Should().Contain("HR; Finance; Ops");
    }

    [Fact]
    public void FormatSpaces_WithNoCategories_ShouldOutputEmptyField()
    {
        // Arrange
        var spaces = new List<Space>
        {
            new()
            {
                Id = 1,
                Name = "Empty",
                Visibility = "public",
                IsCorporate = false,
                IsExternal = false,
                Categories = new List<Category>()
            }
        };

        // Act
        var output = CaptureConsoleOutput(() => _formatter.FormatSpaces(spaces));

        // Assert
        var lines = output.TrimEnd().Split(Environment.NewLine);
        lines[1].Should().EndWith("false,");
    }

    [Fact]
    public void FormatSpaces_ShouldNotIncludeAvatarUrl()
    {
        // Arrange
        var spaces = new List<Space>
        {
            new()
            {
                Id = 1,
                Name = "Test",
                Visibility = "public",
                AvatarUrl = "https://cdn.example.com/space-avatar.png",
                Categories = new List<Category>()
            }
        };

        // Act
        var output = CaptureConsoleOutput(() => _formatter.FormatSpaces(spaces));

        // Assert
        output.Should().NotContain("avatar");
        output.Should().NotContain("https://cdn.example.com/space-avatar.png");
    }

    // ── FormatSpace (single/detail) ───────────────────────────────────

    [Fact]
    public void FormatSpace_ShouldOutputDetailHeaderAndRow()
    {
        // Arrange
        var space = new Space
        {
            Id = 600,
            Name = "Marketing",
            Description = "Marketing team space",
            Visibility = "private",
            IsCorporate = true,
            IsExternal = false,
            IsMandatory = true,
            IsReadOnly = false,
            ParentSpaceId = 100,
            Categories = new List<Category>
            {
                new() { Id = 1, Name = "Brand" }
            },
            CreatedAt = new DateTime(2024, 1, 10, 9, 30, 0),
            UpdatedAt = new DateTime(2024, 6, 20, 14, 0, 0),
            Permalink = "https://workvivo.com/space/600"
        };

        // Act
        var output = CaptureConsoleOutput(() => _formatter.FormatSpace(space));

        // Assert
        var lines = output.TrimEnd().Split(Environment.NewLine);
        lines.Should().HaveCount(2);
        lines[0].Should().Contain("description");
        lines[0].Should().Contain("is_mandatory");
        lines[0].Should().Contain("is_read_only");
        lines[0].Should().Contain("parent_space_id");
        lines[0].Should().Contain("created_at");
        lines[0].Should().Contain("updated_at");
        lines[0].Should().Contain("permalink");
        lines[1].Should().Contain("Marketing team space");
        lines[1].Should().Contain("100");
        lines[1].Should().Contain("Brand");
        lines[1].Should().Contain("2024-01-10 09:30:00");
        lines[1].Should().Contain("2024-06-20 14:00:00");
        lines[1].Should().Contain("https://workvivo.com/space/600");
    }

    [Fact]
    public void FormatSpace_ShouldNotIncludeAvatarUrl()
    {
        // Arrange
        var space = new Space
        {
            Id = 1,
            Name = "Test",
            Visibility = "public",
            AvatarUrl = "https://cdn.example.com/space.png",
            Categories = new List<Category>()
        };

        // Act
        var output = CaptureConsoleOutput(() => _formatter.FormatSpace(space));

        // Assert
        output.Should().NotContain("avatar");
        output.Should().NotContain("https://cdn.example.com/space.png");
    }

    // ── FormatSpaceUsers ──────────────────────────────────────────────

    [Fact]
    public void FormatSpaceUsers_ShouldIncludeSpaceRoleColumn()
    {
        // Arrange
        var users = new List<User>
        {
            new()
            {
                Id = 300,
                ExternalId = "ext-300",
                Email = "charlie@example.com",
                Name = "Charlie Brown",
                DisplayName = "Charlie B.",
                FirstName = "Charlie",
                LastName = "Brown",
                JobTitle = "Designer",
                HasAccess = true,
                SpaceRole = "admin"
            }
        };

        // Act
        var output = CaptureConsoleOutput(() => _formatter.FormatSpaceUsers(users));

        // Assert
        var lines = output.TrimEnd().Split(Environment.NewLine);
        lines.Should().HaveCount(2);
        lines[0].Should().Contain("space_role");
        lines[1].Should().EndWith("admin");
    }

    [Fact]
    public void FormatSpaceUsers_WithNullSpaceRole_ShouldOutputEmptyField()
    {
        // Arrange
        var users = new List<User>
        {
            new()
            {
                Id = 1,
                Email = "test@example.com",
                Name = "Test",
                DisplayName = "Test",
                FirstName = "T",
                LastName = "Est",
                HasAccess = true,
                SpaceRole = null
            }
        };

        // Act
        var output = CaptureConsoleOutput(() => _formatter.FormatSpaceUsers(users));

        // Assert
        var lines = output.TrimEnd().Split(Environment.NewLine);
        lines.Should().HaveCount(2);
        // Last field is null, so row should end with a comma followed by nothing
        lines[1].Should().EndWith("true,");
    }

    [Fact]
    public void FormatSpaceUsers_WithEmptyList_ShouldOutputHeaderOnly()
    {
        // Arrange
        var users = Enumerable.Empty<User>();

        // Act
        var output = CaptureConsoleOutput(() => _formatter.FormatSpaceUsers(users));

        // Assert
        var lines = output.TrimEnd().Split(Environment.NewLine);
        lines.Should().HaveCount(1);
        lines[0].Should().Be("id,external_id,email,name,display_name,first_name,last_name,job_title,has_access,space_role");
    }

    // ── RFC 4180 CSV escaping ─────────────────────────────────────────

    [Fact]
    public void FormatUsers_WithCommaInField_ShouldQuoteField()
    {
        // Arrange
        var users = new List<User>
        {
            new()
            {
                Id = 1,
                Email = "test@example.com",
                Name = "Test",
                DisplayName = "Test",
                FirstName = "T",
                LastName = "Est",
                JobTitle = "VP, Engineering",
                HasAccess = true
            }
        };

        // Act
        var output = CaptureConsoleOutput(() => _formatter.FormatUsers(users));

        // Assert
        output.Should().Contain("\"VP, Engineering\"");
    }

    [Fact]
    public void FormatUsers_WithDoubleQuoteInField_ShouldEscapeWithDoubledQuotes()
    {
        // Arrange
        var users = new List<User>
        {
            new()
            {
                Id = 1,
                Email = "test@example.com",
                Name = "Test",
                DisplayName = "Test \"The Dev\"",
                FirstName = "T",
                LastName = "Est",
                HasAccess = true
            }
        };

        // Act
        var output = CaptureConsoleOutput(() => _formatter.FormatUsers(users));

        // Assert
        // Per RFC 4180, quotes inside a quoted field are doubled
        output.Should().Contain("\"Test \"\"The Dev\"\"\"");
    }

    [Fact]
    public void FormatUsers_WithNewlineInField_ShouldQuoteField()
    {
        // Arrange
        var users = new List<User>
        {
            new()
            {
                Id = 1,
                Email = "test@example.com",
                Name = "Test\nUser",
                DisplayName = "Test",
                FirstName = "T",
                LastName = "Est",
                HasAccess = true
            }
        };

        // Act
        var output = CaptureConsoleOutput(() => _formatter.FormatUsers(users));

        // Assert
        output.Should().Contain("\"Test\nUser\"");
    }

    [Fact]
    public void FormatUsers_WithCarriageReturnInField_ShouldQuoteField()
    {
        // Arrange
        var users = new List<User>
        {
            new()
            {
                Id = 1,
                Email = "test@example.com",
                Name = "Test\rUser",
                DisplayName = "Test",
                FirstName = "T",
                LastName = "Est",
                HasAccess = true
            }
        };

        // Act
        var output = CaptureConsoleOutput(() => _formatter.FormatUsers(users));

        // Assert
        output.Should().Contain("\"Test\rUser\"");
    }

    [Fact]
    public void FormatUsers_WithPlainField_ShouldNotQuoteField()
    {
        // Arrange
        var users = new List<User>
        {
            new()
            {
                Id = 1,
                Email = "plain@example.com",
                Name = "Plain Name",
                DisplayName = "Plain Name",
                FirstName = "Plain",
                LastName = "Name",
                JobTitle = "Developer",
                HasAccess = true
            }
        };

        // Act
        var output = CaptureConsoleOutput(() => _formatter.FormatUsers(users));

        // Assert
        // No field should be wrapped in quotes when content is plain
        var lines = output.TrimEnd().Split(Environment.NewLine);
        lines[1].Should().NotContain("\"");
    }

    [Fact]
    public void FormatSpaces_WithCommaInDescription_ShouldQuoteField()
    {
        // Arrange
        var space = new Space
        {
            Id = 1,
            Name = "Test",
            Description = "A space for engineering, design, and QA",
            Visibility = "public",
            IsCorporate = false,
            IsExternal = false,
            IsMandatory = false,
            IsReadOnly = false,
            Categories = new List<Category>()
        };

        // Act
        var output = CaptureConsoleOutput(() => _formatter.FormatSpace(space));

        // Assert
        output.Should().Contain("\"A space for engineering, design, and QA\"");
    }

    [Fact]
    public void FormatUsers_WithCommaAndQuoteInSameField_ShouldQuoteAndDoubleQuotes()
    {
        // Arrange
        var users = new List<User>
        {
            new()
            {
                Id = 1,
                Email = "test@example.com",
                Name = "Test",
                DisplayName = "Test",
                FirstName = "T",
                LastName = "Est",
                JobTitle = "Director, \"Special Projects\"",
                HasAccess = true
            }
        };

        // Act
        var output = CaptureConsoleOutput(() => _formatter.FormatUsers(users));

        // Assert
        // Should be quoted, with internal quotes doubled
        output.Should().Contain("\"Director, \"\"Special Projects\"\"\"");
    }

    // ── Column count consistency ──────────────────────────────────────

    [Fact]
    public void FormatUsers_HeaderAndRowsShouldHaveSameColumnCount()
    {
        // Arrange
        var users = new List<User>
        {
            new()
            {
                Id = 1,
                Email = "test@example.com",
                Name = "Test",
                DisplayName = "Test",
                FirstName = "T",
                LastName = "Est",
                HasAccess = true
            }
        };

        // Act
        var output = CaptureConsoleOutput(() => _formatter.FormatUsers(users));

        // Assert
        var lines = output.TrimEnd().Split(Environment.NewLine);
        var headerColumns = lines[0].Split(',').Length;
        var rowColumns = lines[1].Split(',').Length;
        headerColumns.Should().Be(rowColumns);
    }

    [Fact]
    public void FormatSpaceUsers_HeaderAndRowsShouldHaveSameColumnCount()
    {
        // Arrange
        var users = new List<User>
        {
            new()
            {
                Id = 1,
                Email = "test@example.com",
                Name = "Test",
                DisplayName = "Test",
                FirstName = "T",
                LastName = "Est",
                HasAccess = true,
                SpaceRole = "member"
            }
        };

        // Act
        var output = CaptureConsoleOutput(() => _formatter.FormatSpaceUsers(users));

        // Assert
        var lines = output.TrimEnd().Split(Environment.NewLine);
        var headerColumns = lines[0].Split(',').Length;
        var rowColumns = lines[1].Split(',').Length;
        headerColumns.Should().Be(rowColumns);
    }

    [Fact]
    public void FormatSpaces_HeaderAndRowsShouldHaveSameColumnCount()
    {
        // Arrange
        var spaces = new List<Space>
        {
            new()
            {
                Id = 1,
                Name = "Test",
                Visibility = "public",
                IsCorporate = false,
                IsExternal = false,
                Categories = new List<Category>()
            }
        };

        // Act
        var output = CaptureConsoleOutput(() => _formatter.FormatSpaces(spaces));

        // Assert
        var lines = output.TrimEnd().Split(Environment.NewLine);
        var headerColumns = lines[0].Split(',').Length;
        var rowColumns = lines[1].Split(',').Length;
        headerColumns.Should().Be(rowColumns);
    }
}
