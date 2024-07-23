using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Bones.Database.DbSets.TaskTracking;

/// <summary>
///     Model for the TaskTracking.Tasks table
/// </summary>
[Table("Tasks", Schema = "TaskTracking")]
[PrimaryKey(nameof(Id))]
public class Task
{
    /// <summary>
    ///     Internal ID for the Task
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    
}