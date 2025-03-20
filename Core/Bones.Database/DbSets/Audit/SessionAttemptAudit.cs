using System.Net;
using Bones.Database.DbConsts;

namespace Bones.Database.DbSets.Audit;

/// <summary>
///     Model for the Audit.SessionAttemptAudits table.
/// </summary>
[Table(TableNames.Audit.SessionAttemptAudits, Schema = SchemaNames.Audit)]
[PrimaryKey(nameof(Id))]
public class SessionAttemptAudit
{
    /// <summary>
    ///   Internal ID for the audit entry
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; init; }

    /// <summary>
    ///   The IP address that attempted to get a session
    /// </summary>
    public required IPAddress IpAddress { get; init; }

    /// <summary>
    ///   The session ID that was attempted to be retrieved
    /// </summary>
    public required Guid SessionId { get; init; }

    /// <summary>
    ///   Whether the attempt was successful
    /// </summary>
    public required bool Successful { get; init; }

    /// <summary>
    ///   When the attempt was made
    /// </summary>
    public required DateTimeOffset AttemptDateTime { get; init; }
} 