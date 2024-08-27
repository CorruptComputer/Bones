using System.Text.Json.Serialization;

namespace Bones.Api.Models;

/// <summary>
///   The response body is empty, this is a workaround for the limitations of the API client.
/// </summary>
[Serializable]
[JsonSerializable(typeof(EmptyResponse))]
public sealed record EmptyResponse
{
    private EmptyResponse() { }

    /// <summary>
    ///   Gets a new empty response
    /// </summary>
    public static readonly EmptyResponse Value = new();
}