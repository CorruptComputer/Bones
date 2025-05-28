using Bones.WebUI.Services.Singleton;

namespace Bones.WebUI.Services.Transient;

/// <summary>
///   Handles unauthorized responses from the API
/// </summary>
/// <param name="authenticationStateProvider"></param>
public class UnauthorizedDelegatingHandler(BonesAuthenticationStateProvider authenticationStateProvider)
    : DelegatingHandler
{
    /// <inheritdoc />
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            await authenticationStateProvider.ClearCurrentUserInBrowserStorageAsync(cancellationToken);
        }

        return response;
    }
}