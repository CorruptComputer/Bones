using Bones.Database.DbSets.ProjectManagement.Layouts;

namespace Bones.Database.DbSets.ProjectManagement.Items;

/// <summary>
///     Model for the ProjectManagement.ItemVersions table
/// </summary>
[Table("ItemVersions", Schema = "ProjectManagement")]
[PrimaryKey(nameof(Id))]
public class ItemVersion
{
    /// <summary>
    ///     Internal ID for the ItemVersion
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    public required LayoutVersion Layout { get; set; }

    public required long Version { get; set; }

    public required Item Item { get; set; }

    public required List<ItemValue> Values { get; set; }
}