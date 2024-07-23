using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Bones.Database.DbSets.TaskTracking;

/// <summary>
///     Model for the TaskTracking.Tags table
/// </summary>
[Table("Tags", Schema = "TaskTracking")]
[PrimaryKey(nameof(Id))]
public class Tag
{
    /// <summary>
    ///     Internal ID for the Tag
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    
}