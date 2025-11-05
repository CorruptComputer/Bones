using Bones.Database.DbConsts;
using Bones.Shared.Backend.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bones.Database.DbSets.Items;


/// <summary>
///   Model for the Item.ItemAssigneeDefinitions table
/// </summary>
[Table(TableNames.Item.ItemAssigneeDefinitions, Schema = SchemaNames.Item)]
[PrimaryKey(nameof(Id))]
public class ItemAssigneeDefinition
{
    /// <summary>
    ///   Internal ID for the ItemAssigneeDefinition
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; init; }

    /// <summary>
    ///   The type of assignment for this assignee definition
    /// </summary>
    public required AssignmentType AssignmentType { get; set; }

    /// <summary>
    ///   The selection type for this assignee definition, single or multiple
    /// </summary>
    public required SelectionType SelectionType { get; set; }

    /// <summary>
    ///   The name to display for this assignee definition
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    ///   The display order for this assignee definition
    /// </summary>
    public required int OrderIndex { get; set; }

    /// <summary>
    ///   Disables creating of new items using this assignee definition,
    ///   and when all items using it are deleted it will be removed.
    /// </summary>
    public bool DeleteFlag { get; set; } = false;

    internal static void BuildTable(EntityTypeBuilder<ItemAssigneeDefinition> builder)
    {
        // Remove deleted items from being included in default queries
        builder.HasQueryFilter(x => !x.DeleteFlag);
    }
}
