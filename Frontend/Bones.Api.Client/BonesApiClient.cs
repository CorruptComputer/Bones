using System.Text.Json;
using System.Text.Json.Serialization;

namespace Bones.Api.Client;

/// <summary>
///   The client for the Bones API.
/// </summary>
/// <param name="httpClientFactory"></param>
public class BonesApiClient(IHttpClientFactory httpClientFactory)
    : AutoGenBonesApiClient(httpClientFactory.CreateClient(HTTP_CLIENT_NAME))
{
    /// <summary>
    ///   The name of the client as registered in the IHttpClientFactory
    /// </summary>
    public const string HTTP_CLIENT_NAME = "BonesApiClient";
}

/// <summary>
///   Partials for the autogenerated client
/// </summary>
public partial class AutoGenBonesApiClient
{
    static partial void UpdateJsonSerializerSettings(JsonSerializerOptions settings)
    {
        settings.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        settings.Converters.Add(new JsonStringEnumConverter());
    }
}