using Workvivo.Shared.Models;

namespace WorkvivoCli.Output;

/// <summary>
/// Outputs data in RFC 4180 CSV format with curated field subsets per model type.
/// Intentionally excludes noisy fields like avatar URLs to keep CSV output clean
/// and focused on actionable data.
/// To add a new model type: add the corresponding <see cref="IOutputFormatter"/>
/// method and define the column set here.
/// </summary>
public class CsvOutputFormatter : IOutputFormatter
{
    public void FormatUsers(IEnumerable<User> users)
    {
        WriteUserListHeader();
        foreach (var user in users)
        {
            WriteUserListRow(user);
        }
    }

    public void FormatUser(User user)
    {
        WriteUserDetailHeader();
        WriteUserDetailRow(user);
    }

    public void FormatSpaces(IEnumerable<Space> spaces)
    {
        WriteSpaceListHeader();
        foreach (var space in spaces)
        {
            WriteSpaceListRow(space);
        }
    }

    public void FormatSpace(Space space)
    {
        WriteSpaceDetailHeader();
        WriteSpaceDetailRow(space);
    }

    public void FormatSpaceUsers(IEnumerable<User> users)
    {
        WriteSpaceUserListHeader();
        foreach (var user in users)
        {
            WriteSpaceUserListRow(user);
        }
    }

    // ── User list (compact) ───────────────────────────────────────────

    private static void WriteUserListHeader()
    {
        Console.WriteLine("id,external_id,email,name,display_name,first_name,last_name,job_title,has_access");
    }

    private static void WriteUserListRow(User user)
    {
        Console.WriteLine(string.Join(",",
            Escape(user.Id.ToString()),
            Escape(user.ExternalId),
            Escape(user.Email),
            Escape(user.Name),
            Escape(user.DisplayName),
            Escape(user.FirstName),
            Escape(user.LastName),
            Escape(user.JobTitle),
            Escape(user.HasAccess ? "true" : "false")));
    }

    // ── Single user detail ────────────────────────────────────────────

    private static void WriteUserDetailHeader()
    {
        Console.WriteLine("id,external_id,email,name,display_name,first_name,last_name,job_title," +
                          "timezone,locale,hire_date,manager_id,has_logged_in,is_frontline,has_access,permalink");
    }

    private static void WriteUserDetailRow(User user)
    {
        Console.WriteLine(string.Join(",",
            Escape(user.Id.ToString()),
            Escape(user.ExternalId),
            Escape(user.Email),
            Escape(user.Name),
            Escape(user.DisplayName),
            Escape(user.FirstName),
            Escape(user.LastName),
            Escape(user.JobTitle),
            Escape(user.Timezone),
            Escape(user.Locale),
            Escape(user.HireDate?.ToString("yyyy-MM-dd")),
            Escape(user.ManagerId?.ToString()),
            Escape(user.HasLoggedIn ? "true" : "false"),
            Escape(user.IsFrontline ? "true" : "false"),
            Escape(user.HasAccess ? "true" : "false"),
            Escape(user.Permalink)));
    }

    // ── Space list (compact) ──────────────────────────────────────────

    private static void WriteSpaceListHeader()
    {
        Console.WriteLine("id,name,visibility,is_corporate,is_external,categories");
    }

    private static void WriteSpaceListRow(Space space)
    {
        var categories = space.Categories.Any()
            ? string.Join("; ", space.Categories.Select(c => c.Name))
            : string.Empty;

        Console.WriteLine(string.Join(",",
            Escape(space.Id.ToString()),
            Escape(space.Name),
            Escape(space.Visibility),
            Escape(space.IsCorporate ? "true" : "false"),
            Escape(space.IsExternal ? "true" : "false"),
            Escape(categories)));
    }

    // ── Single space detail ───────────────────────────────────────────

    private static void WriteSpaceDetailHeader()
    {
        Console.WriteLine("id,name,description,visibility,is_corporate,is_external,is_mandatory," +
                          "is_read_only,parent_space_id,categories,created_at,updated_at,permalink");
    }

    private static void WriteSpaceDetailRow(Space space)
    {
        var categories = space.Categories.Any()
            ? string.Join("; ", space.Categories.Select(c => c.Name))
            : string.Empty;

        Console.WriteLine(string.Join(",",
            Escape(space.Id.ToString()),
            Escape(space.Name),
            Escape(space.Description),
            Escape(space.Visibility),
            Escape(space.IsCorporate ? "true" : "false"),
            Escape(space.IsExternal ? "true" : "false"),
            Escape(space.IsMandatory ? "true" : "false"),
            Escape(space.IsReadOnly ? "true" : "false"),
            Escape(space.ParentSpaceId?.ToString()),
            Escape(categories),
            Escape(space.CreatedAt?.ToString("yyyy-MM-dd HH:mm:ss")),
            Escape(space.UpdatedAt?.ToString("yyyy-MM-dd HH:mm:ss")),
            Escape(space.Permalink)));
    }

    // ── Space users (compact, includes space role) ────────────────────

    private static void WriteSpaceUserListHeader()
    {
        Console.WriteLine("id,external_id,email,name,display_name,first_name,last_name,job_title,has_access,space_role");
    }

    private static void WriteSpaceUserListRow(User user)
    {
        Console.WriteLine(string.Join(",",
            Escape(user.Id.ToString()),
            Escape(user.ExternalId),
            Escape(user.Email),
            Escape(user.Name),
            Escape(user.DisplayName),
            Escape(user.FirstName),
            Escape(user.LastName),
            Escape(user.JobTitle),
            Escape(user.HasAccess ? "true" : "false"),
            Escape(user.SpaceRole)));
    }

    // ── RFC 4180 CSV escaping ─────────────────────────────────────────

    /// <summary>
    /// Escapes a value for CSV output per RFC 4180.
    /// Null values become empty fields. Values containing commas, quotes,
    /// or newlines are wrapped in double-quotes with internal quotes doubled.
    /// </summary>
    private static string Escape(string? value)
    {
        if (value is null)
        {
            return string.Empty;
        }

        if (value.Contains(',') || value.Contains('"') || value.Contains('\n') || value.Contains('\r'))
        {
            return $"\"{value.Replace("\"", "\"\"")}\"";
        }

        return value;
    }
}
