using System.Text.Json;
using System.Text.Json.Serialization;

namespace Bones.Api.Client;

public class BonesApiClient(IHttpClientFactory httpClientFactory)
    : AutoGenBonesApiClient(httpClientFactory.CreateClient(HTTP_CLIENT_NAME))
{
    public const string HTTP_CLIENT_NAME = "BonesApiClient";
}

public partial class AutoGenBonesApiClient
{
    static partial void UpdateJsonSerializerSettings(JsonSerializerOptions settings)
    {
        settings.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        settings.Converters.Add(new JsonStringEnumConverter());
    }
}