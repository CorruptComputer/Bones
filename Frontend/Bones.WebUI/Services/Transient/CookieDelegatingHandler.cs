using Microsoft.AspNetCore.Components.WebAssembly.Http;

namespace Bones.WebUI.Services.Transient;

/// <summary>
///   Sets the cookie on the headers of requests going to the API
/// </summary>
public class CookieDelegatingHandler : DelegatingHandler
{
    /// <inheritdoc />
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
        return await base.SendAsync(request, cancellationToken);
    }
}