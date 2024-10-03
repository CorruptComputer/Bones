using Bones.Database.DbSets.ProjectManagement;

namespace Bones.Database.DbSets.WorkItemManagement;

/// <summary>
///     Model for the WorkItemManagement.WorkItemQueues table
/// </summary>
[Table("WorkItemQueues", Schema = "WorkItemManagement")]
[PrimaryKey(nameof(Id))]
public class WorkItemQueue
{
    /// <summary>
    ///     Internal ID for the Slot
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    /// <summary>
    ///   The initiative this queue belongs to
    /// </summary>
    public required Initiative Initiative { get; set; }

    /// <summary>
    ///   The name of the queue
    /// </summary>
    [MaxLength(512)]
    public required string Name { get; set; }

    /// <summary>
    ///   The work items in this queue
    /// </summary>
    public List<WorkItem> WorkItems { get; set; } = [];

    /// <summary>
    ///   Disables viewing this queue, and when safe to do so it will be removed.
    /// </summary>
    public bool DeleteFlag { get; set; } = false;
}