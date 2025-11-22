namespace Bones.Api.Models.Tasks;

/// <summary>
///   Response model for  actions.
/// </summary>
public sealed record TaskActionResponse
{
    /// <summary>
    ///   The unique identifier of the  action.
    /// </summary>
    public required Guid TaskId { get; init; }

    /// <summary>
    ///   The unique identifier of the current version of the .
    /// </summary>
    public required Guid TaskCurrentVersionId { get; init; }
}
