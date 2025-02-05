namespace Bones.Api.Models.Project;

/// <summary>
///   Request to create a new initiative
/// </summary>
[JsonSerializable(typeof(CreateInitiativeRequest))]
public record CreateInitiativeRequest
{
    /// <summary>
    ///   Name of the initiative to create
    /// </summary>
    [JsonRequired]
    public required string Name { get; init; }
}