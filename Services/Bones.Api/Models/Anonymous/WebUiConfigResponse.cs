namespace Bones.Api.Models.Anonymous;

/// <summary>
///   Response to the GetWebConfigAsync endpoint
/// </summary>
[JsonSerializable(typeof(WebUiConfigResponse))]
public sealed record WebUiConfigResponse
{
    /// <summary>
    ///   Should the test user be prefilled?
    /// </summary>
    [JsonRequired]
    public required bool PrefillTestUser { get; init; }

    /// <summary>
    ///   The version for the API
    /// </summary>
    [JsonRequired]
    public required string ApiVersion { get; init; }
}