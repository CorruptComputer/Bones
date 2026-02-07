using Bones.Database.DbSets.Projects;

namespace Bones.Api.Models.Project;

/// <summary>
///   Response for the GetInitiativesInProjectAsync endpoint
/// </summary>
[JsonSerializable(typeof(GetInitiativesInProjectResponse))]
public record GetInitiativesInProjectResponse
{
    /// <summary>
    ///   The ID of the initiative
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    ///   The name of the initiative
    /// </summary>
    public required string Name { get; init; }

    internal static List<GetInitiativesInProjectResponse> FromInternalList(List<Initiative> initiatives)
    {
        return initiatives.Select(FromInternal).ToList();
    }

    internal static GetInitiativesInProjectResponse FromInternal(Initiative initiative)
    {
        return new()
        {
            Id = initiative.Id,
            Name = initiative.Name
        };
    }
}
