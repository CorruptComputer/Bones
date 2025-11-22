using Bones.Database.DbSets.Items;
using Bones.Shared.Backend.Enums;

namespace Bones.Api.Models.Assignment;

/// <summary>
///   Model representing an assignment slot for an item.
/// </summary>
public sealed record AssignmentSlotModel
{
    /// <summary>
    ///   The unique identifier for the assignment slot.
    /// </summary>
    public required Guid AssignmentSlotId { get; init; }

    /// <summary>
    ///   The display order for this assignee slot
    /// </summary>
    public required int OrderIndex { get; init; }

    /// <summary>
    ///   The name of the assignment slot.
    /// </summary>
    public required string AssignmentSlotName { get; init; }

    /// <summary>
    ///   The level of assignment for this assignee slot, user or role
    /// </summary>
    public required AssignmentType AssignmentSlotLevel { get; set; }

    /// <summary>
    ///   The selection type for this assignee slot, single or multiple
    /// </summary>
    public required SelectionType SelectionSlotType { get; set; }

    /// <summary>
    ///   The states in which this assignment can be
    /// </summary>
    public required List<string> AssignmentStates { get; set; }

    internal static AssignmentSlotModel FromInternal(ItemAssignmentSlot slot)
    {
        return new()
        {
            AssignmentSlotId = slot.Id,
            OrderIndex = slot.OrderIndex,
            AssignmentSlotName = slot.Name,
            AssignmentSlotLevel = slot.AssignmentType,
            SelectionSlotType = slot.SelectionType,
            AssignmentStates = slot.AssignmentStates
        };
    }
}