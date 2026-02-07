using Bones.Database.DbSets.Accounts;
using Bones.Logic.Features.System.SystemSettings;

namespace Bones.Api.Models.SysAdmin;

/// <summary>
///   Response for the SaveSmtpConfigAsync endpoint
/// </summary>
[JsonSerializable(typeof(SaveSmtpConfigRequest))]
public record SaveSmtpConfigRequest
{
    /// <summary>
    ///   Is SMTP enabled?
    /// </summary>
    public required bool IsEnabled { get; init; }

    /// <summary>
    ///   The SMTP server to use for sending emails
    /// </summary>
    public required string Server { get; init; }

    /// <summary>
    ///   The port to use for the SMTP server
    /// </summary>
    public required ushort Port { get; init; }

    /// <summary>
    ///   Whether to use SSL for the connection
    /// </summary>
    public required bool UseSsl { get; init; }

    /// <summary>
    ///   The username to use for the SMTP server
    /// </summary>
    public required string Username { get; init; }

    /// <summary>
    ///   The password to use for the SMTP server
    /// </summary>
    public required string Password { get; init; }

    /// <summary>
    ///   The email address to use as the from address for emails sent by this system
    /// </summary>
    public required string FromAddress { get; init; }

    /// <summary>
    ///   The name to use as the from name for emails sent by this system
    /// </summary>
    public required string FromName { get; init; }

    /// <summary>
    ///   The reason for the change
    /// </summary>
    public required string ChangeReason { get; init; }

    internal SaveSmtpConfig.Command ToInternal(BonesUser user)
    {
        return new(IsEnabled, new()
        {
            Server = Server,
            Port = Port,
            UseSsl = UseSsl,
            Username = Username,
            Password = Password,
            FromAddress = FromAddress,
            FromName = FromName,
        }, ChangeReason, user);
    }
}
