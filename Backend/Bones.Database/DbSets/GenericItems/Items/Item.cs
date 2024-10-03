using Bones.Database.DbSets.GenericItems.ItemLayouts;
using Bones.Database.DbSets.ProjectManagement;

namespace Bones.Database.DbSets.GenericItems.Items;

/// <summary>
///     Model for the GenericItems.Items table
/// </summary>
[Table("Items", Schema = "GenericItems")]
[PrimaryKey(nameof(Id))]
public class Item
{
    /// <summary>
    ///     Internal ID for the Item
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    /// <summary>
    ///   The name of this item
    /// </summary>
    [MaxLength(512)]
    public required string Name { get; set; }

    /// <summary>
    ///   The ID of the project this item belongs to
    /// </summary>
    [ForeignKey(nameof(Project))]
    public required Guid ProjectId { get; set; }

    /// <summary>
    ///   The layout this item will use
    /// </summary>
    public required ItemLayout ItemLayout { get; set; }

    /// <summary>
    ///   The versions for this item
    /// </summary>
    public List<ItemVersion> Versions { get; set; } = [];

    /// <summary>
    ///   The version this item is currently using
    /// </summary>
    public int CurrentVersion { get; set; } = 1;

    /// <summary>
    ///   Disables access to this item and schedules deletion.
    /// </summary>
    public bool DeleteFlag { get; set; } = false;
}