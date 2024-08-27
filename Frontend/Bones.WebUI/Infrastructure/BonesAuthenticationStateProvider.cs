using System.Security.Claims;
using Bones.Api.Client;
using Microsoft.AspNetCore.Components.Authorization;
using ClaimTypes = Bones.Shared.Consts.ClaimTypes;

namespace Bones.WebUI.Infrastructure;

public class BonesAuthenticationStateProvider(LocalStorageService localStorageService) : AuthenticationStateProvider
{
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        GetMyBasicInfoResponse? currentUser = await GetCurrentUserAsync(CancellationToken.None);

        if (currentUser == null)
        {
            return new(new(new ClaimsIdentity()));
        }

        Claim[] claims = [
            new(ClaimTypes.User.EMAIL, currentUser.Email),
            new(ClaimTypes.User.DISPLAY_NAME, currentUser.DisplayName)
        ];

        AuthenticationState? authenticationState = new(new(new ClaimsIdentity(claims, authenticationType: nameof(BonesAuthenticationStateProvider))));

        return authenticationState;
    }

    public async Task SetCurrentUserAsync(GetMyBasicInfoResponse? currentUser, CancellationToken cancellationToken)
    {
        await localStorageService.SetItemAsync(LocalStorageService.CURRENT_USER_KEY, currentUser, cancellationToken);

        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public Task<GetMyBasicInfoResponse?> GetCurrentUserAsync(CancellationToken cancellationToken)
    {
        return localStorageService.GetItemAsync<GetMyBasicInfoResponse>(LocalStorageService.CURRENT_USER_KEY, cancellationToken);
    }
}