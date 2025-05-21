namespace Bones.Api.Models.WorkItems;

/// <summary>
///   Request to create a new work item
/// </summary>
[JsonSerializable(typeof(CreateWorkItemRequest))]
public sealed record CreateWorkItemRequest
{
    /// <summary>
    ///   The name of the work item
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    ///   The ID of the work item layout
    /// </summary>
    public required Guid WorkItemLayoutId { get; init; }

    /// <summary>
    ///   The fields of the work item
    /// </summary>
    public required List<ItemValueModel> FieldValues { get; init; }


}
