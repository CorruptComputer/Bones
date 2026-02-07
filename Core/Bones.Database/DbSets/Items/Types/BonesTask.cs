using Bones.Database.DbConsts;
using Bones.Database.DbSets.Projects;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bones.Database.DbSets.Items.Types;

/// <summary>
///   Model for the Items.Types.Tasks table
/// </summary>
[Table(TableNames.Item.Types.Tasks, Schema = SchemaNames.Items_Types)]
[PrimaryKey(nameof(Id))]
public class BonesTask // I'd really like to just call this 'Task' but C# said no :(
{
    /// <summary>
    ///   Internal ID for the Task
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; init; }

    /// <summary>
    ///   The ID of the queue this task belongs to
    /// </summary>
    public required Guid TaskQueueId { get; set; }

    /// <summary>
    ///   The DateTime this item was added to the queue
    /// </summary>
    public required DateTimeOffset AddedToQueueDateTime { get; set; }

    /// <summary>
    ///   The ID of the item for this task
    /// </summary>
    public required Guid ItemId { get; init; }

    /// <summary>
    ///   Disables viewing this item, and when safe to do so it will be removed.
    /// </summary>
    public bool DeleteFlag { get; set; } = false;

    #region Navigational Properties
    /// <summary>
    ///   Navigational property for the queue this task belongs to, null if not .Include()'d in the query
    /// </summary>
    public TaskQueue? TaskQueue { get; set; }

    /// <summary>
    ///   Navigational property for the item this task is for, null if not .Include()'d in the query
    /// </summary>
    public Item? Item { get; set; }
    #endregion

    internal static void BuildTable(EntityTypeBuilder<BonesTask> builder)
    {
        // Remove deleted items from being included in default queries
        builder.HasQueryFilter(x => !x.DeleteFlag);

        builder.HasOne(t => t.TaskQueue)
            .WithMany(tq => tq.BonesTasks)
            .HasForeignKey(t => t.TaskQueueId);

        builder.HasOne(t => t.Item)
            .WithOne()
            .HasForeignKey<BonesTask>(t => t.ItemId);
    }
}