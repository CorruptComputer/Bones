namespace Bones.Api.Models;

/// <summary>
/// 
/// </summary>
public sealed record ApiConfiguration
{
    /// <summary>
    /// 
    /// </summary>
    public string? WebUIBaseUrl { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string? ApiBaseUrl { get; set; }
}