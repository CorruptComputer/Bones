using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Bones.Database.DbSets;

/// <summary>
///     Model for the TaskHistory table.
/// </summary>
[Table("TaskHistory")]
[PrimaryKey(nameof(Id))]
public class TaskHistory
{
    /// <summary>
    ///     Internal ID for the Task History.
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    /// <summary>
    ///     Name of the Task, should usually be set to nameof(ClassName)
    /// </summary>
    public required string TaskName { get; set; }

    /// <summary>
    ///     Time the task run was started
    /// </summary>
    public required DateTimeOffset StartDateTime { get; set; }


    /// <summary>
    ///     Time the task run was ended, DateTimeOffset.MaxValue if DNF, or null if running.
    /// </summary>
    public DateTimeOffset? EndDateTime { get; set; }

    /// <summary>
    ///     Did this task finish with an error?
    /// </summary>
    public bool? Errored { get; set; }

    /// <summary>
    ///     Reason or cause of the error.
    /// </summary>
    public string? ErrorMessage { get; set; }
}