using Bones.Database.DbConsts;
using Bones.Database.DbSets.Items;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bones.Database.DbSets.TaskManagement;

/// <summary>
///   Model for the TaskManagement.Tasks table
/// </summary>
[Table(TableNames.Task.Tasks, Schema = SchemaNames.TaskManagement)]
[PrimaryKey(nameof(Id))]
public class BonesTask // I'd really like to just call this 'Task' but C# said no :(
{
    /// <summary>
    ///   Internal ID for the Task
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    /// <summary>
    ///   The queue this task belongs to
    /// </summary>
    public required TaskQueue TaskQueue { get; set; }

    /// <summary>
    ///   The DateTime this item was added to the queue
    /// </summary>
    public required DateTimeOffset AddedToQueueDateTime { get; set; }

    /// <summary>
    ///   The item for this task
    /// </summary>
    public required Item Item { get; set; }

    /// <summary>
    ///   Disables viewing this item, and when safe to do so it will be removed.
    /// </summary>
    public bool DeleteFlag { get; set; } = false;

    internal static void BuildTable(EntityTypeBuilder<BonesTask> builder)
    {
        // Remove deleted items from being included in default queries
        builder.HasQueryFilter(x => !x.DeleteFlag);
    }
}