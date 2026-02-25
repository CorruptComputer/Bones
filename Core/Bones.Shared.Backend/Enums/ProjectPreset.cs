using System.Text.Json.Serialization;

namespace Bones.Shared.Backend.Enums;

/// <summary>
///   The presets available for a new project
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ProjectPreset
{
    /// <summary>
    ///   No preset should be applied to this project
    /// </summary>
    None,
    /// <summary>
    ///   Preset for a development project
    /// </summary>
    Development,

    /// <summary>
    ///   Preset for a test project
    /// </summary>
    Test,

    //InformationTechnology,
    //HomeManagement
}
