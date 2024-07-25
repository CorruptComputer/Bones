namespace Bones.Database.DbSets.ProjectManagement.Layouts;

/// <summary>
///     Model for the ProjectManagement.ItemLayouts table
/// </summary>
[Table("ItemLayouts", Schema = "ProjectManagement")]
[PrimaryKey(nameof(Id))]
public class Layout
{
    /// <summary>
    ///     Internal ID for the ItemLayout Version
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; init; }

    [MaxLength(512)]
    public required string Name { get; init; }

    /// <summary>
    ///   The versions for this layout
    /// </summary>
    public List<LayoutVersion> Versions { get; set; } = [];
}