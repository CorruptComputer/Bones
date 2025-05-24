namespace Bones.Api.Models.Anonymous;

/// <summary>
///   Response to the GetWebConfigAsync endpoint
/// </summary>
[JsonSerializable(typeof(ApiConfigResponse))]
public sealed record ApiConfigResponse
{
    /// <summary>
    ///   Is the API setup for testing?
    /// </summary>
    [JsonRequired]
    public required bool SetupForTesting { get; init; }

    /// <summary>
    ///   The version for the API
    /// </summary>
    [JsonRequired]
    public required string ApiVersion { get; init; }
}