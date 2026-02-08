using System.ComponentModel;
using Spectre.Console.Cli;

namespace WorkvivoCli.Commands;

/// <summary>
/// Base settings shared by all commands.
/// Add global options here — they will be inherited by every command.
/// </summary>
public class GlobalSettings : CommandSettings
{
    [CommandOption("--json")]
    [Description("Output in JSON format")]
    [DefaultValue(false)]
    public bool Json { get; init; }

    [CommandOption("--csv")]
    [Description("Output in CSV format")]
    [DefaultValue(false)]
    public bool Csv { get; init; }
}
