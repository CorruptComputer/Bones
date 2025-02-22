namespace Bones.Api.Models.SysAdmin;

/// <summary>
///   
/// </summary>
[JsonSerializable(typeof(GetSystemAdminDashboardResponse))]
public record GetSystemAdminDashboardResponse
{
    /// <summary>
    ///   The number of users in the system
    /// </summary>
    public int UserCount { get; init; }

    /// <summary>
    ///   The number of organizations in the system
    /// </summary>
    public int OrganizationCount { get; init; }

    /// <summary>
    ///   The number of projects in the system
    /// </summary>
    public int ProjectCount { get; init; }

    /// <summary>
    ///   The number of items in the system
    /// </summary>
    public int ItemCount { get; init; }
}
