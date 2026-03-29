namespace WorkvivoCli.Output;

/// <summary>
/// Supported output formats for CLI commands.
/// Add new formats here and implement a corresponding <see cref="IOutputFormatter"/>.
/// </summary>
public enum OutputFormat
{
    Table,
    Json,
    Csv
}
