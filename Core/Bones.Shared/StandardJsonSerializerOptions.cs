using System.Text.Json;
using System.Text.Json.Serialization;

namespace Bones.Shared;

/// <summary>
///   Standard JSON serializer options
/// </summary>
public static class StandardJsonSerializerOptions
{
    /// <summary>
    ///   The default JSON serializer options to be used
    /// </summary>
    public static JsonSerializerOptions Default => new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        AllowTrailingCommas = true,
        RespectNullableAnnotations = true
    };
}
