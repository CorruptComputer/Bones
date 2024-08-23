namespace Bones.Database.DbSets.ProjectManagement;

/// <summary>
///     Model for the ProjectManagement.ItemLayoutVersions table
/// </summary>
[Table("ItemLayoutVersions", Schema = "ProjectManagement")]
[PrimaryKey(nameof(Id))]
public class ItemLayoutVersion
{
    /// <summary>
    ///     Internal ID for the ItemLayoutVersion
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; init; }

    /// <summary>
    ///   The layout this version belongs to
    /// </summary>
    public required ItemLayout ItemLayout { get; init; }

    /// <summary>
    ///   The version number for this layout
    /// </summary>
    public required long Version { get; init; }

    public List<ItemField> Fields { get; init; } = [];

    /// <summary>
    ///   Disables creating of new items using this layout version,
    ///   and when all items using it are deleted it will be removed.
    /// </summary>
    public bool DeleteFlag { get; set; } = false;
}