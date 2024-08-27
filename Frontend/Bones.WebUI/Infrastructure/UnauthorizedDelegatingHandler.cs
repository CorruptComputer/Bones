using Bones.Api.Client;

namespace Bones.WebUI.Infrastructure;

public class UnauthorizedDelegatingHandler(BonesAuthenticationStateProvider authenticationStateProvider)
    : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            await authenticationStateProvider.SetCurrentUserAsync(null, cancellationToken);
        }

        return response;
    }
}