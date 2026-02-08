using System.Text.Json.Serialization;

namespace WorkvivoCli.Models;

public class User
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("external_id")]
    public string? ExternalId { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("first_name")]
    public string FirstName { get; set; } = string.Empty;

    [JsonPropertyName("last_name")]
    public string LastName { get; set; } = string.Empty;

    [JsonPropertyName("display_name")]
    public string DisplayName { get; set; } = string.Empty;

    [JsonPropertyName("avatar_url")]
    public string? AvatarUrl { get; set; }

    [JsonPropertyName("job_title")]
    public string? JobTitle { get; set; }

    [JsonPropertyName("timezone")]
    public string? Timezone { get; set; }

    [JsonPropertyName("locale")]
    public string? Locale { get; set; }

    [JsonPropertyName("hire_date")]
    public DateTime? HireDate { get; set; }

    [JsonPropertyName("manager_id")]
    public long? ManagerId { get; set; }

    [JsonPropertyName("date_of_birth")]
    public DateTime? DateOfBirth { get; set; }

    [JsonPropertyName("mobile_phone")]
    public string? MobilePhone { get; set; }

    [JsonPropertyName("direct_dial")]
    public string? DirectDial { get; set; }

    [JsonPropertyName("has_logged_in")]
    public bool HasLoggedIn { get; set; }

    [JsonPropertyName("is_frontline")]
    public bool IsFrontline { get; set; }

    [JsonPropertyName("has_access")]
    public bool HasAccess { get; set; }

    [JsonPropertyName("permalink")]
    public string? Permalink { get; set; }

    [JsonPropertyName("space_role")]
    public string? SpaceRole { get; set; }
}
