using Bones.Database.DbSets.ProjectManagement.WorkItems;

namespace Bones.Database.DbSets.ProjectManagement;

/// <summary>
///     Model for the ProjectManagement.Queues table
/// </summary>
[Table("Queues", Schema = "ProjectManagement")]
[PrimaryKey(nameof(Id))]
public class Queue
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