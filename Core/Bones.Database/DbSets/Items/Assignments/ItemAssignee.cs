using Bones.Database.DbConsts;
using Bones.Database.DbSets.Accounts;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bones.Database.DbSets.Items.Assignments;

/// <summary>
///   Model for the Items.Assignments.ItemAssignees table
/// </summary>
[Table(TableNames.Item.Assignments.ItemAssignees, Schema = SchemaNames.Items_Assignments)]
[PrimaryKey(nameof(Id))]
public class ItemAssignee
{
    /// <summary>
    ///   Internal ID for the ItemAssignee
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; init; }

    /// <summary>
    ///   The ID of the slot this assignee belongs to
    /// </summary>
    public required Guid ItemAssignmentSlotId { get; set; }

    /// <summary>
    ///   The ID of the ItemVersion this assignee is for
    /// </summary>
    public required Guid ItemVersionId { get; set; }

    /// <summary>
    ///   The ID of the user assigned to this, if applicable
    /// </summary>
    public Guid? AssignedUserId { get; set; }

    /// <summary>
    ///   The ID of the role assigned to this, if applicable
    /// </summary>
    public Guid? AssignedRoleId { get; set; }

    /// <summary>
    ///   The state of this assignment
    /// </summary>
    public required string State { get; set; }

    /// <summary>
    ///   Disables creating of new items using this assignee,
    ///   and when all items using it are deleted it will be removed.
    /// </summary>
    public bool DeleteFlag { get; set; } = false;

    #region Navigational Properties
    /// <summary>
    ///   Navigational property for the slot this assignee is in, null if not .Include()'d in the query
    /// </summary>
    public ItemAssignmentSlot? ItemAssignmentSlot { get; set; }

    /// <summary>
    ///   Navigational property for the ItemVersion this assignee is for, null if not .Include()'d in the query
    /// </summary>
    public ItemVersion? ItemVersion { get; set; }

    /// <summary>
    ///   Navigational property for the user in this assignee, null if not .Include()'d in the query or unassigned
    /// </summary>
    public BonesUser? AssignedUser { get; set; }

    /// <summary>
    ///   Navigational property for the role in this assignee, null if not .Include()'d in the query or unassigned
    /// </summary>
    public BonesRole? AssignedRole { get; set; }
    #endregion

    internal static void BuildTable(EntityTypeBuilder<ItemAssignee> builder)
    {
        // Remove deleted items from being included in default queries
        builder.HasQueryFilter(ia => !ia.DeleteFlag);

        builder.HasOne(ia => ia.ItemAssignmentSlot)
            .WithMany()
            .HasForeignKey(ia => ia.ItemAssignmentSlotId);

        builder.HasOne(ia => ia.ItemVersion)
            .WithMany(iv => iv.ItemAssignees)
            .HasForeignKey(ia => ia.ItemVersionId);

        builder.HasOne(ia => ia.AssignedUser)
            .WithMany()
            .HasForeignKey(ia => ia.AssignedUserId);

        builder.HasOne(ia => ia.AssignedRole)
            .WithMany()
            .HasForeignKey(ia => ia.AssignedRoleId);
    }
}
