using Bones.Database.DbSets.Items.Assignments;

namespace Bones.Api.Models.Assignment;

/// <summary>
///   Model representing an assignee.
/// </summary>
public sealed record AssigneeModel
{
    /// <summary>
    ///   The unique identifier for the assignee.
    /// </summary>
    public required Guid AssigneeId { get; init; }

    /// <summary>
    ///   The unique identifier for the assignment slot this assignee is assigned to.
    /// </summary>
    public required Guid AssignmentSlotId { get; init; }

    /// <summary>
    ///   The user ID assigned to this assignee, if applicable.
    /// </summary>
    public required Guid? AssignedUserId { get; init; }

    /// <summary>
    ///   The role ID assigned to this assignee, if applicable.
    /// </summary>
    public required Guid? AssignedRoleId { get; init; }

    /// <summary>
    ///   The display name of the assignee.
    /// </summary>
    public required string AssigneeDisplayName { get; init; }

    /// <summary>
    ///   The state in which this assignment is in
    /// </summary>
    public required string AssignmentState { get; set; }

    internal static AssigneeModel FromInternal(ItemAssignee assignee)
    {
        string displayName;
        if (assignee.AssignedUser is not null)
        {
            displayName = assignee.AssignedUser.DisplayName ?? assignee.AssignedUser.Email!;
        }
        else if (assignee.AssignedRole is not null)
        {
            displayName = $"Role: {assignee.AssignedRole.Name}";
        }
        else
        {
            throw new InvalidOperationException("Assignee must have either an assigned user or role.");
        }

        return new()
        {
            AssigneeId = assignee.Id,
            AssignmentSlotId = assignee.ItemAssignmentSlotId,
            AssignedUserId = assignee.AssignedUserId,
            AssignedRoleId = assignee.AssignedRoleId,
            AssigneeDisplayName = displayName,
            AssignmentState = assignee.State
        };
    }
}