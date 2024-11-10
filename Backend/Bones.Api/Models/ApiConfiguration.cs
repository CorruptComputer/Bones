namespace Bones.Api.Models;

/// <summary>
///   Configuration for the API project
/// </summary>
internal sealed record ApiConfiguration
{
    /// <summary>
    ///   The origins we want to allow through CORS
    /// </summary>
    public string[]? CorsAllowedOrigins { get; init; }
}