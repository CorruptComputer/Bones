namespace Bones.WebUI.Infrastructure;

public class UnauthorizedDelegatingHandler(BonesAuthenticationStateProvider authenticationStateProvider)
    : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {

        HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            var currentUser = await authenticationStateProvider.GetCurrentUserAsync();

            if(currentUser != null)
            {
                await authenticationStateProvider.SetCurrentUserAsync(null);
            }
        }

        return response;
    }
}