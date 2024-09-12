namespace Bones.Database.DbSets.ProjectManagement;

/// <summary>
///     Model for the ProjectManagement.Initiatives table
/// </summary>
[Table("Initiatives", Schema = "ProjectManagement")]
[PrimaryKey(nameof(Id))]
public class Initiative
{
    /// <summary>
    ///     Internal ID for the Tag
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    /// <summary>
    ///   The project this initiative belongs to
    /// </summary>
    public required Project Project { get; set; }

    /// <summary>
    ///   The name of this initiative
    /// </summary>
    [MaxLength(512)]
    public required string Name { get; set; }

    /// <summary>
    ///   The child queues for this initiative 
    /// </summary>
    public List<Queue> Queues { get; set; } = [];

    /// <summary>
    ///   Disables access to this Initiative and schedules deletes for everything within,
    ///   when all items using it are deleted it will be removed.
    /// </summary>
    public bool DeleteFlag { get; set; } = false;
}