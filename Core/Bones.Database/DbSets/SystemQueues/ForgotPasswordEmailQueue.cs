namespace Bones.Database.DbSets.SystemQueues;

/// <summary>
///     Model for the SystemQueues.ForgotPasswordEmailQueue table
/// </summary>
[Table("ForgotPasswordEmailQueue", Schema = "SystemQueues")]
[PrimaryKey(nameof(Id))]
public class ForgotPasswordEmailQueue
{
    /// <summary>
    ///     Internal ID for the Queue item
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    /// <summary>
    ///   The time this queue item was created at
    /// </summary>
    public DateTimeOffset Created { get; set; } = DateTimeOffset.Now;

    /// <summary>
    ///   Number of times this has failed and retried.
    /// </summary>
    public int RetryCount { get; set; } = 0;

    /// <summary>
    ///   The last time this was tried and failed, if it hasn't been tried: null
    /// </summary>
    public DateTimeOffset? LastTry { get; set; }

    /// <summary>
    ///   The reason it has failed each time
    /// </summary>
    public List<string> FailureReasons { get; set; } = [];

    /// <summary>
    ///   Who is this email going to
    /// </summary>
    [MaxLength(512)]
    public required string EmailTo { get; set; }

    /// <summary>
    ///   What is the link to set for the reset
    /// </summary>
    [MaxLength(4096)]
    public required string PasswordResetLink { get; set; }
}