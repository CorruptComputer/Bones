using Bones.Shared.Backend.Enums;

namespace Bones.Api.Models.Item;

/// <summary>
///   Slot of an item assignee
/// </summary>
public class ItemAssigneeSlotModel
{
    /// <summary>
    ///   The order number of the assignee slot
    /// </summary>
    [JsonRequired]
    public required int OrderNumber { get; init; }

    /// <summary>
    ///   Name of the assignee slot
    /// </summary>
    [JsonRequired]
    public required string Name { get; init; }

    /// <summary>
    ///   The assignment type
    /// </summary>
    [JsonRequired]
    public required AssignmentType AssignmentType { get; init; }

    /// <summary>
    ///   The selection type
    /// </summary>
    [JsonRequired]
    public required SelectionType SelectionType { get; init; }

    /// <summary>
    ///   The states this assignment can be in
    /// </summary>
    [JsonRequired]
    public required List<string> States { get; init; }
}
