namespace Bones.Backend.Models;

/// <summary>
///   Configuration for the backend of the project
/// </summary>
public sealed record BackendConfiguration
{
    /// <summary>
    ///   Base URL for the Web UI
    /// </summary>
    public string? WebUIBaseUrl { get; set; }
}