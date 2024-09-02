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

    /// <summary>
    ///   The time this error occurred at
    /// </summary>
    public required DateTimeOffset ErrorTime { get; set; }

    /// <summary>
    ///   The error message
    /// </summary>
    public required string ErrorMessage { get; set; }

    /// <summary>
    ///   The stacktrace of the error
    /// </summary>
    public required string StackTrace { get; set; }
}