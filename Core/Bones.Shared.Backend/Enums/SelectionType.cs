using System.Text.Json.Serialization;

namespace Bones.Shared.Backend.Enums;

/// <summary>
///   The selection type
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SelectionType
{
    /// <summary>
    ///   Single selection only
    /// </summary>
    Single,

    /// <summary>
    ///   Multiple selections allowed
    /// </summary>
    Multiple
}
