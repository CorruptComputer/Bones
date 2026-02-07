using Bones.Database.DbConsts;
using Bones.Database.DbSets.Items.Layouts;
using Bones.Shared.Backend.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bones.Database.DbSets.Items.Assignments;

/// <summary>
///   Model for the Items.Assignments.ItemAssignmentSlots table
/// </summary>
[Table(TableNames.Item.Assignments.ItemAssignmentSlots, Schema = SchemaNames.Items_Assignments)]
[PrimaryKey(nameof(Id))]
public class ItemAssignmentSlot
{
    /// <summary>
    ///   Internal ID for the ItemAssignmentSlot
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; init; }

    /// <summary>
    ///   The type of assignment for this assignee slot, user or role
    /// </summary>
    public required AssignmentType AssignmentType { get; set; }

    /// <summary>
    ///   The selection type for this assignee slot, single or multiple
    /// </summary>
    public required SelectionType SelectionType { get; set; }

    /// <summary>
    ///   The name to display for this assignee slot
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    ///   The display order for this assignee slot
    /// </summary>
    public required int OrderIndex { get; set; }

    /// <summary>
    ///   The states in which this assignment can be
    /// </summary>
    public required List<string> AssignmentStates { get; set; }

    /// <summary>
    ///   The ID of the ItemLayoutVersion this slot is for
    /// </summary>
    public required Guid ItemLayoutVersionId { get; init; }

    /// <summary>
    ///   Disables creating of new items using this assignee slot,
    ///   and when all items using it are deleted it will be removed.
    /// </summary>
    public bool DeleteFlag { get; set; } = false;

    #region Navigational Properties
    /// <summary>
    ///   Navigational property to the ItemLayoutVersion this slot belongs to, null if not .Include()'d in the query
    /// </summary>
    public ItemLayoutVersion? ItemLayoutVersion { get; init; }
    #endregion

    internal static void BuildTable(EntityTypeBuilder<ItemAssignmentSlot> builder)
    {
        // Remove deleted items from being included in default queries
        builder.HasQueryFilter(ias => !ias.DeleteFlag);

        builder.HasOne(ias => ias.ItemLayoutVersion)
               .WithMany(ilv => ilv.ItemAssignmentSlots)
               .HasForeignKey(ias => ias.ItemLayoutVersionId);
    }
}
