using Bones.Database.DbSets.GenericItems.Items;

namespace Bones.Database.DbSets.WorkItemManagement;

/// <summary>
///     Model for the WorkItemManagement.WorkItems table
/// </summary>
[Table("WorkItems", Schema = "WorkItemManagement")]
[PrimaryKey(nameof(Id))]
public class WorkItem
{
    /// <summary>
    ///     Internal ID for the WorkItem
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    /// <summary>
    ///   The queue this work item belongs to
    /// </summary>
    public required WorkItemQueue WorkItemQueue { get; set; }

    /// <summary>
    ///   The DateTime this item was added to the queue
    /// </summary>
    public required DateTimeOffset AddedToQueueDateTime { get; set; }

    /// <summary>
    ///   The generic item for this work item
    /// </summary>
    public required Item Item { get; set; }

    /// <summary>
    ///   Disables viewing this item, and when safe to do so it will be removed.
    /// </summary>
    public bool DeleteFlag { get; set; } = false;
}