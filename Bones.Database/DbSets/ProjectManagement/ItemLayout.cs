namespace Bones.Database.DbSets.ProjectManagement;

/// <summary>
///     Model for the ProjectManagement.ItemLayouts table
/// </summary>
[Table("ItemLayouts", Schema = "ProjectManagement")]
[PrimaryKey(nameof(Id))]
public class ItemLayout
{
    /// <summary>
    ///     Internal ID for the ItemLayout Version
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; init; }

    [MaxLength(512)]
    public required string Name { get; set; }

    /// <summary>
    ///   The versions for this layout
    /// </summary>
    public List<ItemLayoutVersion> Versions { get; set; } = [];

    /// <summary>
    ///   Disables creating of new items using this layout,
    ///   and when all items using it are deleted it will be removed.
    /// </summary>
    public bool DeleteFlag { get; set; } = false;
}