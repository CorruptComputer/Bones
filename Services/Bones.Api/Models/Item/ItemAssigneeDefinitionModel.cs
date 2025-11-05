using Bones.Shared.Backend.Enums;

namespace Bones.Api.Models.Item;

/// <summary>
///   Definition of an item assignee
/// </summary>
public class ItemAssigneeDefinitionModel
{
    /// <summary>
    ///   The order number of the assignee definition
    /// </summary>
    [JsonRequired]
    public required int OrderNumber { get; init; }

    /// <summary>
    ///   Name of the assignee definition
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
}
