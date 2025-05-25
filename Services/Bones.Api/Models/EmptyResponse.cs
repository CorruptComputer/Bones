namespace Bones.Api.Models;

/// <summary>
///   This is a workaround for the limitations of the API client.
/// </summary>
[Serializable]
[JsonSerializable(typeof(EmptyResponse))]
public sealed record EmptyResponse
{
    /// <summary>
    ///   Without something in the body the API client will not generate anything for this type.
    ///   Don't actually have to put anything in it.
    /// </summary>
    public string? Workaround { get; init; }

    private EmptyResponse() { }

    /// <summary>
    ///   Gets a new empty response
    /// </summary>
    [JsonIgnore]
    public static readonly EmptyResponse Value = new();
}