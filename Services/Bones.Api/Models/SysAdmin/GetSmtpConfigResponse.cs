using Bones.Database.Operations.System.SystemSettings.Models;

namespace Bones.Api.Models.SysAdmin;

/// <summary>
///   Response for the GetSmtpConfigAsync endpoint
/// </summary>
[JsonSerializable(typeof(GetSmtpConfigResponse))]
public record GetSmtpConfigResponse
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

    internal static GetSmtpConfigResponse FromInternal(bool enabled, SmtpConfig? config)
    {
        return new()
        {
            IsEnabled = enabled,
            Server = config?.Server ?? string.Empty,
            Port = config?.Port ?? 25,
            UseSsl = config?.UseSsl ?? true,
            Username = config?.Username ?? string.Empty,
            Password = config?.Password ?? string.Empty,
            FromAddress = config?.FromAddress ?? string.Empty,
            FromName = config?.FromName ?? string.Empty
        };
    }
}
