using Bones.Api.Client;

namespace Bones.WebUI.Infrastructure;

public class UnauthorizedDelegatingHandler(BonesAuthenticationStateProvider authenticationStateProvider, BonesApiClient apiClient)
    : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {

        HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            await apiClient.AccountManagementLogoutAsync(cancellationToken);
            await authenticationStateProvider.SetCurrentUserAsync(null, cancellationToken);
        }

        return response;
    }
}