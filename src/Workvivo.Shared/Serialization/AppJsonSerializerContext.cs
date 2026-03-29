using System.Text.Json.Serialization;
using Workvivo.Shared.Models;

namespace Workvivo.Shared.Serialization;

/// <summary>
/// JSON serialization context for trim-safe JSON operations.
/// This uses source generators to pre-generate serialization code,
/// eliminating the need for reflection and making the app trim-safe.
/// </summary>
[JsonSerializable(typeof(User))]
[JsonSerializable(typeof(List<User>))]
[JsonSerializable(typeof(IEnumerable<User>))]
[JsonSerializable(typeof(Space))]
[JsonSerializable(typeof(List<Space>))]
[JsonSerializable(typeof(IEnumerable<Space>))]
[JsonSerializable(typeof(PagedResponse<User>))]
[JsonSerializable(typeof(PagedResponse<Space>))]
[JsonSerializable(typeof(SingleItemResponse<User>))]
[JsonSerializable(typeof(SingleItemResponse<Space>))]
[JsonSourceGenerationOptions(
    WriteIndented = true,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
public partial class AppJsonSerializerContext : JsonSerializerContext
{
}
