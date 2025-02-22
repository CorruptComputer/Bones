namespace Bones.Database.Operations.System.SystemSettings.Models;

/// <summary>
///   System settings for SMTP connections
/// </summary>
public class SmtpConfig
{
    /// <summary>
    ///   The SMTP server to use for sending emails
    /// </summary>
    public string? Server { get; set; }

    /// <summary>
    ///   The port to use for the SMTP server
    /// </summary>
    public ushort? Port { get; set; }

    /// <summary>
    ///   Whether to use SSL for the connection
    /// </summary>
    public bool? UseSsl { get; set; }

    /// <summary>
    ///   The username to use for the SMTP server
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    ///   The password to use for the SMTP server
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    ///   The email address to use as the from address for emails sent by this system
    /// </summary>
    public string? FromAddress { get; set; }

    /// <summary>
    ///   The name to use as the from name for emails sent by this system
    /// </summary>
    public string? FromName { get; set; }
}
