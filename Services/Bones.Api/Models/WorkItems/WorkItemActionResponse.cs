namespace Bones.Api.Models.WorkItems;

/// <summary>
///   Response model for work item actions.
/// </summary>
public sealed record WorkItemActionResponse
{
    /// <summary>
    ///   The unique identifier of the work item action.
    /// </summary>
    public required Guid WorkItemId { get; init; }

    /// <summary>
    ///   The unique identifier of the current version of the work item.
    /// </summary>
    public required Guid WorkItemCurrentVersionId { get; init; }
}
