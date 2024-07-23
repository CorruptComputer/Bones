using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Bones.Database.DbSets.TaskTracking;

/// <summary>
///     Model for the TaskTracking.Queues table
/// </summary>
[Table("Queues", Schema = "TaskTracking")]
[PrimaryKey(nameof(Id))]
public class Queue
{
    /// <summary>
    ///     Internal ID for the Slot
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    
}