namespace Bones.Api.Models.Project;

/// <summary>
///   Request to create a new project
/// </summary>
[JsonSerializable(typeof(CreateProjectRequest))]
public record CreateProjectRequest
{
    /// <summary>
    ///   Name of the project to create
    /// </summary>
    [JsonRequired]
    public required string Name { get; init; }

    /// <summary>
    ///   Optionally the organization that this should be created under, if not specified will be created for the requesting user.
    /// </summary>
    public Guid? OrganizationId { get; init; }
}