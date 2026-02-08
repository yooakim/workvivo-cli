using FluentAssertions;
using WorkvivoCli.Commands;
using WorkvivoCli.Output;
using Xunit;

namespace WorkvivoCli.Tests.Output;

public class OutputFormatterFactoryTests
{
    [Fact]
    public void Create_WithNoFlags_ShouldReturnTableOutputFormatter()
    {
        // Arrange
        var settings = new GlobalSettings { Json = false, Csv = false };

        // Act
        var formatter = OutputFormatterFactory.Create(settings);

        // Assert
        formatter.Should().BeOfType<TableOutputFormatter>();
    }

    [Fact]
    public void Create_WithJsonFlag_ShouldReturnJsonOutputFormatter()
    {
        // Arrange
        var settings = new GlobalSettings { Json = true, Csv = false };

        // Act
        var formatter = OutputFormatterFactory.Create(settings);

        // Assert
        formatter.Should().BeOfType<JsonOutputFormatter>();
    }

    [Fact]
    public void Create_WithCsvFlag_ShouldReturnCsvOutputFormatter()
    {
        // Arrange
        var settings = new GlobalSettings { Json = false, Csv = true };

        // Act
        var formatter = OutputFormatterFactory.Create(settings);

        // Assert
        formatter.Should().BeOfType<CsvOutputFormatter>();
    }

    [Fact]
    public void Create_WithBothJsonAndCsvFlags_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var settings = new GlobalSettings { Json = true, Csv = true };

        // Act
        var act = () => OutputFormatterFactory.Create(settings);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*one output format*");
    }

    [Fact]
    public void IsMachineReadable_WithNoFlags_ShouldReturnFalse()
    {
        // Arrange
        var settings = new GlobalSettings { Json = false, Csv = false };

        // Act
        var result = OutputFormatterFactory.IsMachineReadable(settings);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsMachineReadable_WithJsonFlag_ShouldReturnTrue()
    {
        // Arrange
        var settings = new GlobalSettings { Json = true, Csv = false };

        // Act
        var result = OutputFormatterFactory.IsMachineReadable(settings);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsMachineReadable_WithCsvFlag_ShouldReturnTrue()
    {
        // Arrange
        var settings = new GlobalSettings { Json = false, Csv = true };

        // Act
        var result = OutputFormatterFactory.IsMachineReadable(settings);

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData(false, false, OutputFormat.Table)]
    [InlineData(true, false, OutputFormat.Json)]
    [InlineData(false, true, OutputFormat.Csv)]
    public void ResolveFormat_ShouldReturnExpectedFormat(bool json, bool csv, OutputFormat expected)
    {
        // Arrange
        var settings = new GlobalSettings { Json = json, Csv = csv };

        // Act
        var format = OutputFormatterFactory.ResolveFormat(settings);

        // Assert
        format.Should().Be(expected);
    }

    [Fact]
    public void ResolveFormat_WithConflictingFlags_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var settings = new GlobalSettings { Json = true, Csv = true };

        // Act
        var act = () => OutputFormatterFactory.ResolveFormat(settings);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*one output format*");
    }
}
