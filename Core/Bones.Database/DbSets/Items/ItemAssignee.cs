using Bones.Database.DbConsts;
using Bones.Database.DbSets.AccountManagement;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bones.Database.DbSets.Items;

/// <summary>
///   Model for the Item.ItemAssignees table
/// </summary>
[Table(TableNames.Item.ItemAssignees, Schema = SchemaNames.Item)]
[PrimaryKey(nameof(Id))]
public class ItemAssignee
{
    /// <summary>
    ///   Internal ID for the ItemAssignee
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; init; }

    /// <summary>
    ///   The slot this assignee is in
    /// </summary>
    public required ItemAssignmentSlot Slot { get; set; }

    /// <summary>
    ///   The user assigned to this assignee, if applicable
    /// </summary>
    public required BonesUser? AssignedUser { get; set; }

    /// <summary>
    ///   The role assigned to this assignee, if applicable
    /// </summary>
    public required BonesRole? AssignedRole { get; set; }

    /// <summary>
    ///   The state of this assignment
    /// </summary>
    public required string State { get; set; }

    /// <summary>
    ///   Disables creating of new items using this assignee,
    ///   and when all items using it are deleted it will be removed.
    /// </summary>
    public bool DeleteFlag { get; set; } = false;

    internal static void BuildTable(EntityTypeBuilder<ItemAssignmentSlot> builder)
    {
        // Remove deleted items from being included in default queries
        builder.HasQueryFilter(x => !x.DeleteFlag);
    }
}
