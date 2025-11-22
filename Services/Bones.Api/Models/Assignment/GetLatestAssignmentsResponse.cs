using System;
using Bones.Database.DbSets.Items;
using Bones.Shared.Backend.Enums;

namespace Bones.Api.Models.Assignment;

/// <summary>
///   Response model for getting the latest assignments for an item.
/// </summary>
[JsonSerializable(typeof(GetLatestAssignmentsResponse))]
public sealed record GetLatestAssignmentsResponse
{
    /// <summary>
    ///   The assignment slots associated with the item.
    /// </summary>
    public required List<AssignmentSlotModel> AssignmentSlots { get; init; }

    /// <summary>
    ///   The assignees currently assigned to the item.
    /// </summary>
    public required List<AssigneeModel> Assignees { get; init; }

    internal static GetLatestAssignmentsResponse FromInternal(List<ItemAssignmentSlot> assigneeSlots, List<ItemAssignee> assignees)
    {
        return new()
        {
            AssignmentSlots = [.. assigneeSlots.Select(AssignmentSlotModel.FromInternal)],
            Assignees = [.. assignees.Select(AssigneeModel.FromInternal)]
        };
    }
}
