using Bones.Api.Client.AutoGen;
using ReQuesty.Runtime.Abstractions.Authentication;
using ReQuesty.Runtime.Http;

namespace Bones.Api.Client;

/// <summary>
///   The client for the Bones API.
/// </summary>
/// <remarks>
///   Create an instance with a preconfigured <see cref="HttpClient"/>.
/// </remarks>
/// <param name="httpClient"></param>
public class BonesApiClient(HttpClient httpClient)
    : AutoGenBonesApiClient(new HttpClientRequestAdapter(new AnonymousAuthenticationProvider(), httpClient: httpClient))
{
    /// <summary>
    ///   Create an instance with a preconfigured <see cref="HttpClient"/> from the <see cref="IHttpClientFactory"/>
    /// </summary>
    /// <param name="httpClientFactory"></param>
    public BonesApiClient(IHttpClientFactory httpClientFactory)
        : this(httpClientFactory.CreateClient(HTTP_CLIENT_NAME)) { }

    /// <summary>
    ///   The name of the client as registered in the IHttpClientFactory
    /// </summary>
    public const string HTTP_CLIENT_NAME = "BonesApiClient";
}