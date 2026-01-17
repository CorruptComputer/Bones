using Bones.Database.DbConsts;
using Bones.Database.DbSets.ProjectManagement;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bones.Database.DbSets.TaskManagement;

/// <summary>
///   Model for the TaskManagement.TaskQueues table
/// </summary>
[Table(TableNames.Task.TaskQueues, Schema = SchemaNames.TaskManagement)]
[PrimaryKey(nameof(Id))]
public class TaskQueue
{
    /// <summary>
    ///   Internal ID for the Slot
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    /// <summary>
    ///   The ID of the initiative this queue belongs to
    /// </summary>
    public required Guid InitiativeId { get; set; }

    /// <summary>
    ///   The name of the queue
    /// </summary>
    [MaxLength(512)]
    public required string Name { get; set; }

    /// <summary>
    ///   Disables viewing this queue, and when safe to do so it will be removed.
    /// </summary>
    public bool DeleteFlag { get; set; } = false;

    #region Navigational Properties
    /// <summary>
    ///   Navigational property for the initiative that owns this queue, null if not .Include()'d in the query
    /// </summary>
    public Initiative? Initiative { get; set; }

    /// <summary>
    ///   Navigational property for the tasks in this queue, empty if not .Include()'d in the query
    /// </summary>
    public List<BonesTask> BonesTasks { get; set; } = [];
    #endregion

    internal static void BuildTable(EntityTypeBuilder<TaskQueue> builder)
    {
        // Remove deleted items from being included in default queries
        builder.HasQueryFilter(tq => !tq.DeleteFlag);

        builder.HasOne(tq => tq.Initiative)
            .WithMany(i => i.Queues)
            .HasForeignKey(tq => tq.InitiativeId);

        builder.HasMany(tq => tq.BonesTasks)
            .WithOne(t => t.TaskQueue)
            .HasForeignKey(t => t.TaskQueueId);
    }
}