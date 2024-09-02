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

    /// <summary>
    ///   The name of the work item
    /// </summary>
    [MaxLength(512)]
    public required string Name { get; set; }

    /// <summary>
    ///   The parent queue this work item belongs to
    /// </summary>
    public required Queue Queue { get; set; }

    /// <summary>
    ///   The DateTime this item was added to the queue
    /// </summary>
    public required DateTimeOffset AddedToQueueDateTime { get; set; }

    /// <summary>
    ///   The tags this work item has
    /// </summary>
    public List<Tag> Tags { get; set; } = [];

    /// <summary>
    ///   The versions this work item has
    /// </summary>
    public List<WorkItemVersion> Versions { get; set; } = [];

    /// <summary>
    ///   Disables viewing this item, and when safe to do so it will be removed.
    /// </summary>
    public bool DeleteFlag { get; set; } = false;
}