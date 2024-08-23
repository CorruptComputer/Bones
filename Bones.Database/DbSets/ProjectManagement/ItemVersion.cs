namespace Bones.Database.DbSets.ProjectManagement;

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

    public required ItemLayoutVersion ItemLayout { get; set; }

    public required long Version { get; set; }

    public required Item Item { get; set; }

    public required List<ItemValue> Values { get; set; }

    /// <summary>
    ///   Disables viewing this item version, and when safe to do so it will be removed.
    /// </summary>
    public bool DeleteFlag { get; set; } = false;
}