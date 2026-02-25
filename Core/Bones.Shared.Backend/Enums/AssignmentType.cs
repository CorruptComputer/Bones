using System.Text.Json.Serialization;

namespace Bones.Shared.Backend.Enums;

/// <summary>
///   The assignment type
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AssignmentType
{
    /// <summary>
    ///   Assigned to a user
    /// </summary>
    User,

    /// <summary>
    ///   Assigned to a role, or group of users
    /// </summary>
    Role
}
