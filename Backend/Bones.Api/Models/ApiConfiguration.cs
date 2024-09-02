namespace Bones.Api.Models;

/// <summary>
///   Configuration for the API project
/// </summary>
internal sealed record ApiConfiguration
{
    /// <summary>
    ///   Base URL for the WebUI that uses this API.
    /// </summary>
    public string? WebUIBaseUrl { get; set; }

    /// <summary>
    ///   Base URL for the API.
    /// </summary>
    public string? ApiBaseUrl { get; set; }
}