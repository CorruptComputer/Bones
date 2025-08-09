using Bones.Shared.Enums;

namespace Bones.Api.Models.Project;

/// <summary>
///   Request to get the projects for a given User/Organization
/// </summary>
[JsonSerializable(typeof(GetProjectsByOwnerRequest))]
public record GetProjectsByOwnerRequest
{
    /// <summary>
    ///   OwnerType to get
    /// </summary>
    [JsonRequired]
    public required OwnershipType OwnerType { get; init; }

    /// <summary>
    ///   Optionally the organization that this should be created under, if not specified will be created for the requesting user.
    /// </summary>
    public Guid? OrganizationId { get; init; }
}