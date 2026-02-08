using System.Text.Json.Serialization;

namespace WorkvivoCli.Models;

public class Space
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("parent_space_id")]
    public long? ParentSpaceId { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("avatar_url")]
    public string? AvatarUrl { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("visibility")]
    public string Visibility { get; set; } = string.Empty;

    [JsonPropertyName("is_external")]
    public bool IsExternal { get; set; }

    [JsonPropertyName("is_corporate")]
    public bool IsCorporate { get; set; }

    [JsonPropertyName("is_mandatory")]
    public bool IsMandatory { get; set; }

    [JsonPropertyName("is_read_only")]
    public bool IsReadOnly { get; set; }

    [JsonPropertyName("categories")]
    public List<Category> Categories { get; set; } = new();

    [JsonPropertyName("created_at")]
    public DateTime? CreatedAt { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    [JsonPropertyName("permalink")]
    public string? Permalink { get; set; }
}

public class Category
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime? CreatedAt { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTime? UpdatedAt { get; set; }
}
