using Bones.Database.DbSets.ProjectManagement.Items;

namespace Bones.Database.DbSets.ProjectManagement.Layouts;

/// <summary>
///     Model for the ProjectManagement.ItemLayoutVersions table
/// </summary>
[Table("ItemLayoutVersions", Schema = "ProjectManagement")]
[PrimaryKey(nameof(Id))]
public class LayoutVersion
{
    /// <summary>
    ///     Internal ID for the ItemLayoutVersion
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; init; }

    /// <summary>
    ///   The layout this version belongs to
    /// </summary>
    public required Layout Layout { get; init; }

    /// <summary>
    ///   The version number for this layout
    /// </summary>
    public required long Version { get; init; }

    public List<ItemField> Fields { get; init; } = [];
}