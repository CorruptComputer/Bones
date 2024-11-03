using System.Security.Claims;
using Bones.Api.Client;
using Bones.Shared.Consts;
using Microsoft.AspNetCore.Components.Authorization;

namespace Bones.WebUI.Infrastructure;

/// <summary>
///   Provides the state of authentication
/// </summary>
/// <param name="localStorageService"></param>
public class BonesAuthenticationStateProvider(LocalStorageService localStorageService) : AuthenticationStateProvider
{
    /// <inheritdoc />
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        GetMyBasicInfoResponse? currentUser = await GetCurrentUserAsync(CancellationToken.None);

        if (currentUser == null)
        {
            return new(new(new ClaimsIdentity()));
        }

        Claim[] claims = [
            new(BonesClaimTypes.User.EMAIL, currentUser.Email ?? string.Empty),
            new(BonesClaimTypes.User.DISPLAY_NAME, currentUser.DisplayName ?? string.Empty)
        ];

        AuthenticationState authenticationState = new(new(new ClaimsIdentity(claims, authenticationType: nameof(BonesAuthenticationStateProvider))));

        return authenticationState;
    }

    /// <summary>
    ///   Sets the current user
    /// </summary>
    /// <param name="currentUser"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task SetCurrentUserAsync(GetMyBasicInfoResponse currentUser, CancellationToken cancellationToken)
    {
        await localStorageService.SetItemAsync(LocalStorageService.CURRENT_USER_KEY, currentUser, cancellationToken);

        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    /// <summary>
    ///   Clears the current user
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task ClearCurrentUserAsync(CancellationToken cancellationToken)
    {
        await localStorageService.RemoveItemAsync(LocalStorageService.CURRENT_USER_KEY, cancellationToken);

        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    /// <summary>
    ///   Gets the current user
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<GetMyBasicInfoResponse?> GetCurrentUserAsync(CancellationToken cancellationToken)
    {
        return localStorageService.GetItemAsync<GetMyBasicInfoResponse>(LocalStorageService.CURRENT_USER_KEY, cancellationToken);
    }
}