using System.Text.Json.Serialization;

namespace WorkvivoCli.Models;

public class PagedResponse<T>
{
    [JsonPropertyName("data")]
    public List<T> Data { get; set; } = new();

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("meta")]
    public MetaData? Meta { get; set; }

    // Convenience properties
    public int Skip => Meta?.Pagination?.Skip ?? 0;
    public int Take => Meta?.Pagination?.Take ?? 0;
    public int Total => Meta?.Pagination?.TotalRecords ?? Data.Count;
}

public class SingleItemResponse<T>
{
    [JsonPropertyName("data")]
    public T? Data { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
}

public class MetaData
{
    [JsonPropertyName("pagination")]
    public PaginationInfo? Pagination { get; set; }
}

public class PaginationInfo
{
    [JsonPropertyName("skip")]
    public int Skip { get; set; }

    [JsonPropertyName("take")]
    public int Take { get; set; }

    [JsonPropertyName("total_records")]
    public int? TotalRecords { get; set; }

    [JsonPropertyName("next_page")]
    public string? NextPage { get; set; }
}
