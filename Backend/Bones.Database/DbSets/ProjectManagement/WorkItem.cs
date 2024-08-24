namespace Bones.Database.DbSets.ProjectManagement;

/// <summary>
///     Model for the ProjectManagement.WorkItems table
/// </summary>
[Table("WorkItems", Schema = "ProjectManagement")]
[PrimaryKey(nameof(Id))]
public class WorkItem
{
    /// <summary>
    ///     Internal ID for the WorkItem
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [MaxLength(512)]
    public required string Name { get; set; }

    public required Queue Queue { get; set; }

    public required DateTimeOffset AddedToQueueDateTime { get; set; }

    public List<Tag> Tags { get; set; } = [];

    public List<WorkItemVersion> Versions { get; set; } = [];

    /// <summary>
    ///   Disables viewing this item, and when safe to do so it will be removed.
    /// </summary>
    public bool DeleteFlag { get; set; } = false;
}