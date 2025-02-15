using System;
using Bones.Shared.Consts;
using Microsoft.AspNetCore.Components;

namespace Bones.WebUI.Infrastructure;

/// <summary>
///   Handles forbidden responses from the API
/// </summary>
/// <param name="navigationManager"></param>
public class ForbiddenDelegatingHandler(NavigationManager navigationManager)
    : DelegatingHandler
{
    /// <inheritdoc />
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
        {
            navigationManager.NavigateTo(FrontEndUrls.HOME);
        }

        return response;
    }
}