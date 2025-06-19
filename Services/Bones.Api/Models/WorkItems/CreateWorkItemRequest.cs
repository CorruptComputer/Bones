namespace Bones.Api.Models.WorkItems;

/// <summary>
///   Request to create a new work item
/// </summary>
[JsonSerializable(typeof(CreateWorkItemRequest))]
public sealed record CreateWorkItemRequest
{
    /// <summary>
    ///   The ID of the work item layout (not version, automatically uses the current version)
    /// </summary>
    public required Guid WorkItemLayoutId { get; init; }

    /// <summary>
    ///   The title of the work item
    /// </summary>
    public required string Title { get; init; }

    /// <summary>
    ///   The fields of the work item
    /// </summary>
    public required List<ItemValueModel> FieldValues { get; init; }
}
