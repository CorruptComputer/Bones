using System.Net;
using Bones.Database.DbConsts;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bones.Database.DbSets.AccountManagement;

/// <summary>
///   Model for the AccountManagement.BonesUserSessions table.
/// </summary>
[Table(TableNames.AccountManagement.BonesUserSessions, Schema = SchemaNames.AccountManagement)]
[PrimaryKey(nameof(Id))]
public class BonesUserSession
{
    /// <summary>
    ///   Internal ID for the user session
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; init; }

    /// <summary>
    ///   The ID of the user this session is for
    /// </summary>
    public required Guid UserId { get; init; }

    /// <summary>
    ///   The IP address that the session was created from, if it changes the session should be invalidated
    /// </summary>
    public required IPAddress IpAddress { get; init; }

    /// <summary>
    ///   The encryption key to use for browser localStorage
    /// </summary>
    [MaxLength(64)] // Only really needs 44, but might as well give it a little buffer room in case I want to increase the key size later
    public required string Base64LocalStorageKey { get; init; }

    /// <summary>
    ///   The date and time the session was created
    /// </summary>
    public required DateTimeOffset CreatedDateTime { get; init; }

    /// <summary>
    ///   The date and time the session was last accessed
    ///   TODO: at some point this should be timed out and invalidated, maybe 30 days?
    /// </summary>
    public DateTimeOffset? LastAccessedDateTime { get; set; }

    /// <summary>
    ///   If the session has been invalidated
    /// </summary>
    public bool IsInvalidated { get; set; } = false;

    #region Navigational Properties
    /// <summary>
    ///   Navigation property for the user this session is for, null if not .Include()'d in the query
    /// </summary>
    public BonesUser? User { get; }
    #endregion

    internal static void BuildTable(EntityTypeBuilder<BonesUserSession> builder)
    {
        // Go ahead and remove these from being included in default queries
        builder.HasQueryFilter(x => !x.IsInvalidated);

        builder.HasOne(s => s.User)
               .WithMany()
               .HasForeignKey(s => s.UserId);
    }
}