namespace Bones.Database.DbSets.SystemQueues;

/// <summary>
///     Model for the SystemQueues.ConfirmationEmailDeadQueue table
/// </summary>
[Table("ConfirmationEmailDeadQueue", Schema = "SystemQueues")]
[PrimaryKey(nameof(Id))]
public class ConfirmationEmailDeadQueue
{
    /// <summary>
    ///     Internal ID for the dead Queue item
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    /// <summary>
    ///   The time this queue item was created at in this dead queue
    /// </summary>
    public DateTimeOffset DeadQueueCreated { get; set; } = DateTimeOffset.Now;

    /// <summary>
    ///   The time this queue item was created at in the original queue
    /// </summary>
    public required DateTimeOffset OriginalCreated { get; set; }

    /// <summary>
    ///   Number of times this has failed and retried.
    /// </summary>
    public required int RetryCount { get; set; }

    /// <summary>
    ///   The last time this was tried and failed
    /// </summary>
    public required DateTimeOffset LastTry { get; set; }

    /// <summary>
    ///   The reason it has failed each time
    /// </summary>
    public required List<string> FailureReasons { get; set; }

    /// <summary>
    ///   Who is this email going to
    /// </summary>
    [MaxLength(512)]
    public required string EmailTo { get; set; }

    /// <summary>
    ///   What is the link to set for the confirmation
    /// </summary>
    [MaxLength(4096)]
    public required string ConfirmationLink { get; set; }

    internal static ConfirmationEmailDeadQueue FromConfirmationEmailQueue(ConfirmationEmailQueue queueItem)
    {
        return new()
        {
            OriginalCreated = queueItem.Created,
            ConfirmationLink = queueItem.ConfirmationLink,
            RetryCount = queueItem.RetryCount,
            LastTry = queueItem.LastTry ?? DateTimeOffset.Now,
            DeadQueueCreated = DateTimeOffset.Now,
            EmailTo = queueItem.EmailTo,
            FailureReasons = queueItem.FailureReasons
        };
    }
}