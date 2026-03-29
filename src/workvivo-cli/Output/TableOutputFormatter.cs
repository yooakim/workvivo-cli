using Spectre.Console;
using Workvivo.Shared.Models;

namespace WorkvivoCli.Output;

public class TableOutputFormatter : IOutputFormatter
{
    public void FormatUsers(IEnumerable<User> users)
    {
        var table = new Table();
        table.Border = TableBorder.Rounded;
        table.AddColumn("ID");
        table.AddColumn("Name");
        table.AddColumn("Email");
        table.AddColumn("Job Title");
        table.AddColumn("Access");

        foreach (var user in users)
        {
            table.AddRow(
                user.Id.ToString(),
                user.DisplayName ?? user.Name,
                user.Email,
                user.JobTitle ?? "N/A",
                user.HasAccess ? "Yes" : "No");
        }

        AnsiConsole.Write(table);
    }

    public void FormatUser(User user)
    {
        var table = new Table();
        table.Border = TableBorder.Rounded;
        table.AddColumn("Property");
        table.AddColumn("Value");

        table.AddRow("ID", user.Id.ToString());
        table.AddRow("External ID", user.ExternalId ?? "N/A");
        table.AddRow("Display Name", user.DisplayName ?? "N/A");
        table.AddRow("Name", user.Name ?? "N/A");
        table.AddRow("First Name", user.FirstName ?? "N/A");
        table.AddRow("Last Name", user.LastName ?? "N/A");
        table.AddRow("Email", user.Email ?? "N/A");
        table.AddRow("Job Title", user.JobTitle ?? "N/A");
        table.AddRow("Timezone", user.Timezone ?? "N/A");
        table.AddRow("Locale", user.Locale ?? "N/A");
        table.AddRow("Has Logged In", user.HasLoggedIn ? "Yes" : "No");
        table.AddRow("Is Frontline", user.IsFrontline ? "Yes" : "No");
        table.AddRow("Has Access", user.HasAccess ? "Yes" : "No");

        if (user.HireDate.HasValue)
        {
            table.AddRow("Hire Date", user.HireDate.Value.ToString("yyyy-MM-dd"));
        }
        if (user.ManagerId.HasValue)
        {
            table.AddRow("Manager ID", user.ManagerId.Value.ToString());
        }
        if (!string.IsNullOrWhiteSpace(user.Permalink))
        {
            table.AddRow("Permalink", user.Permalink);
        }

        AnsiConsole.Write(table);
    }

    public void FormatSpaces(IEnumerable<Space> spaces)
    {
        var table = new Table();
        table.Border = TableBorder.Rounded;
        table.AddColumn("ID");
        table.AddColumn("Name");
        table.AddColumn("Visibility");
        table.AddColumn("Type");
        table.AddColumn("Categories");

        foreach (var space in spaces)
        {
            var type = space.IsCorporate ? "Corporate" : "Community";
            var categories = space.Categories.Any()
                ? string.Join(", ", space.Categories.Select(c => c.Name))
                : "None";

            table.AddRow(
                space.Id.ToString(),
                space.Name,
                space.Visibility,
                type,
                categories);
        }

        AnsiConsole.Write(table);
    }

    public void FormatSpace(Space space)
    {
        var table = new Table();
        table.Border = TableBorder.Rounded;
        table.AddColumn("Property");
        table.AddColumn("Value");

        table.AddRow("ID", space.Id.ToString());
        table.AddRow("Name", space.Name ?? "N/A");
        table.AddRow("Description", space.Description ?? "N/A");
        table.AddRow("Visibility", space.Visibility ?? "N/A");
        table.AddRow("Is Corporate", space.IsCorporate ? "Yes" : "No");
        table.AddRow("Is External", space.IsExternal ? "Yes" : "No");
        table.AddRow("Is Mandatory", space.IsMandatory ? "Yes" : "No");
        table.AddRow("Is Read Only", space.IsReadOnly ? "Yes" : "No");

        if (space.ParentSpaceId.HasValue)
        {
            table.AddRow("Parent Space ID", space.ParentSpaceId.Value.ToString());
        }

        if (space.Categories.Any())
        {
            table.AddRow("Categories", string.Join(", ", space.Categories.Select(c => c.Name)));
        }

        if (space.CreatedAt.HasValue)
        {
            table.AddRow("Created At", space.CreatedAt.Value.ToString("yyyy-MM-dd HH:mm:ss"));
        }
        if (space.UpdatedAt.HasValue)
        {
            table.AddRow("Updated At", space.UpdatedAt.Value.ToString("yyyy-MM-dd HH:mm:ss"));
        }
        if (!string.IsNullOrWhiteSpace(space.Permalink))
        {
            table.AddRow("Permalink", space.Permalink);
        }

        AnsiConsole.Write(table);
    }

    public void FormatSpaceUsers(IEnumerable<User> users)
    {
        var table = new Table();
        table.Border = TableBorder.Rounded;
        table.AddColumn("ID");
        table.AddColumn("Name");
        table.AddColumn("Email");
        table.AddColumn("Job Title");
        table.AddColumn("Space Role");

        foreach (var user in users)
        {
            table.AddRow(
                user.Id.ToString(),
                user.DisplayName ?? user.Name,
                user.Email,
                user.JobTitle ?? "N/A",
                user.SpaceRole ?? "N/A");
        }

        AnsiConsole.Write(table);
    }
}
