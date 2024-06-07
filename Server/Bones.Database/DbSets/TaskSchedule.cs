using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Bones.Database.DbSets;

/// <summary>
///     Model for the TaskSchedule table.
/// </summary>
[Table("TaskSchedule")]
[PrimaryKey(nameof(TaskScheduleId))]
public class TaskSchedule
{
    /// <summary>
    ///     Internal ID for the Task Schedule.
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long TaskScheduleId { get; set; }

    /// <summary>
    ///     Name of the Task, should usually be set to nameof(ClassName)
    /// </summary>
    [MaxLength(200)]
    public required string TaskName { get; set; }

    /// <summary>
    ///     The last time this task finished successfully, or null if it has not run.
    /// </summary>
    public DateTimeOffset? LastRunTime { get; set; }

    /// <summary>
    ///     Time after which this task will be eligible to run again, or null if it should not run again.
    /// </summary>
    public DateTimeOffset? NextRunTime { get; set; }

    /// <summary>
    ///     Is this task running?
    /// </summary>
    public required bool Running { get; set; } = false;
}