using System.Text.Json;
using System.Text.Json.Serialization;

namespace Bones.WebUI.ApiClients;

public class BonesApiClient(IHttpClientFactory httpClientFactory)
    : AutoGenBonesApiClient(httpClientFactory.CreateClient(API_CLIENT_NAME))
{
    public const string API_CLIENT_NAME = "BonesApiClient";
}

public partial class AutoGenBonesApiClient
{
    static partial void UpdateJsonSerializerSettings(JsonSerializerOptions settings)
    {
        settings.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        settings.Converters.Add(new JsonStringEnumConverter());
    }

    partial void ProcessResponse(HttpClient client, HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            // Need to fix .NETs broke ass AddIdentityApiEndpoints method, it just straight up lies about what these endpoints return.
            // These return a header to set the authentication cookie, with an empty body.
            // Why the hell it says they return an AccessTokenResponse, I have no idea.
            // Ask whoever wrote these:
            // https://github.com/dotnet/aspnetcore/blob/main/src/Identity/Core/src/IdentityApiEndpointRouteBuilderExtensions.cs#L80
            if (response.RequestMessage?.RequestUri?.AbsolutePath == "/Auth/login"
                || response.RequestMessage?.RequestUri?.AbsolutePath == "/Auth/refresh")
            {
                AccessTokenResponse atr = new()
                {
                    TokenType = "Cookie",
                    AccessToken = string.Empty,
                    ExpiresIn = 0,
                    RefreshToken = string.Empty,
                };
                
                response.Content = new StringContent(JsonSerializer.Serialize(atr));
            }
        }
    }
}