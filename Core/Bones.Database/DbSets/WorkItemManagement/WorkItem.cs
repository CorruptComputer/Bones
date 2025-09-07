using System.Runtime.Serialization;
using Bones.Database.DbConsts;
using Bones.Database.DbSets.GenericItems;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bones.Database.DbSets.WorkItemManagement;

/// <summary>
///   Model for the WorkItemManagement.WorkItems table
/// </summary>
[Table(TableNames.WorkItemManagement.WorkItems, Schema = SchemaNames.WorkItemManagement)]
[PrimaryKey(nameof(Id))]
public class WorkItem
{
    /// <summary>
    ///   Internal ID for the WorkItem
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
    public required GenericItem Item { get; set; }

    /// <summary>
    ///   The current version of the generic item this work item is using
    /// </summary>
    [IgnoreDataMember]
    public GenericItemVersion? CurrentVersion => Item.Versions
        .FirstOrDefault(x => x.Version == Item.CurrentVersion);

    /// <summary>
    ///   Disables viewing this item, and when safe to do so it will be removed.
    /// </summary>
    public bool DeleteFlag { get; set; } = false;

    internal static void BuildTable(EntityTypeBuilder<WorkItem> builder)
    {
        // Remove deleted items from being included in default queries
        builder.HasQueryFilter(x => !x.DeleteFlag);
    }
}