namespace Bones.Database.DbSets.System;

/// <summary>
///     Model for the System.TaskErrors table
/// </summary>
[Table("TaskErrors", Schema = "System")]
[PrimaryKey(nameof(Id))]
public class TaskError
{
    /// <summary>
    ///     Internal ID for the Task Error
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    public required DateTimeOffset ErrorTime { get; set; }

    public required string ErrorMessage { get; set; }

    public required string StackTrace { get; set; }
}