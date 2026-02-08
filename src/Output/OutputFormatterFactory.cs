using WorkvivoCli.Commands;

namespace WorkvivoCli.Output;

/// <summary>
/// Resolves the appropriate <see cref="IOutputFormatter"/> based on the output
/// format flags in <see cref="GlobalSettings"/>.
/// To add a new format: add the flag to <see cref="GlobalSettings"/>, add an
/// <see cref="OutputFormat"/> enum value, register the formatter here, and
/// implement <see cref="IOutputFormatter"/>.
/// </summary>
public static class OutputFormatterFactory
{
    /// <summary>
    /// Creates the appropriate <see cref="IOutputFormatter"/> for the given settings.
    /// </summary>
    /// <param name="settings">The command settings containing output format flags.</param>
    /// <returns>An <see cref="IOutputFormatter"/> instance matching the requested format.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when multiple output format flags are specified simultaneously.
    /// </exception>
    public static IOutputFormatter Create(GlobalSettings settings)
    {
        var format = ResolveFormat(settings);

        return format switch
        {
            OutputFormat.Json => new JsonOutputFormatter(),
            OutputFormat.Csv => new CsvOutputFormatter(),
            OutputFormat.Table => new TableOutputFormatter(),
            _ => throw new InvalidOperationException($"Unsupported output format: {format}")
        };
    }

    /// <summary>
    /// Resolves the <see cref="OutputFormat"/> from the flags on <see cref="GlobalSettings"/>.
    /// Validates that at most one output format flag is set.
    /// </summary>
    public static OutputFormat ResolveFormat(GlobalSettings settings)
    {
        var flagCount = 0;
        if (settings.Json)
            flagCount++;
        if (settings.Csv)
            flagCount++;

        if (flagCount > 1)
        {
            throw new InvalidOperationException(
                "Only one output format flag can be specified at a time (e.g., --json or --csv, not both).");
        }

        if (settings.Json)
            return OutputFormat.Json;
        if (settings.Csv)
            return OutputFormat.Csv;
        return OutputFormat.Table;
    }

    /// <summary>
    /// Returns <c>true</c> when the resolved format is a machine-readable format
    /// (JSON, CSV, etc.) rather than a human-oriented table.
    /// Useful for deciding whether to suppress informational stderr messages.
    /// </summary>
    public static bool IsMachineReadable(GlobalSettings settings)
    {
        var format = ResolveFormat(settings);
        return format is OutputFormat.Json or OutputFormat.Csv;
    }
}
