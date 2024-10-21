namespace Bones.BackgroundService.Models;

/// <summary>
///   Configuration for the backend of the project
/// </summary>
public sealed record BackgroundServiceConfiguration
{
    /// <summary>
    ///   The email address the background service user should use, this is also what will appear in emails to users
    /// </summary>
    public string? BackgroundServiceUserEmail { get; set; }

    /// <summary>
    ///   If the user already exists, and you'd like to change its email. Set the ID here
    /// </summary>
    public string? BackgroundServiceUserId { get; set; }

    /// <summary>
    ///   The IP/Domain that should be used to connect to the SMTP server
    /// </summary>
    public string? SmtpServer { get; set; }

    /// <summary>
    ///   The port that should be used to connect to the SMTP server
    /// </summary>
    public int? SmtpPort { get; set; }

    /// <summary>
    ///   The user, if any, that should be used to connect to the SMTP server
    /// </summary>
    public string? SmtpUser { get; set; }

    /// <summary>
    ///   The password, if any, that should be used to connect to the SMTP server
    /// </summary>
    public string? SmtpPassword { get; set; }
}