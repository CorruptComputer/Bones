using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace Bones.WebUI.Infrastructure;

public class BonesAuthenticationStateProvider(LocalStorageService localStorageService) : AuthenticationStateProvider
{
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        object? currentUser = await GetCurrentUserAsync(CancellationToken.None);

        if (currentUser == null)
        {
            return new(new(new ClaimsIdentity()));
        }

        Claim[] claims = [
            //new(ClaimTypes.Email, currentUser.Email)
        ];

        AuthenticationState? authenticationState = new(new(new ClaimsIdentity(claims, authenticationType: nameof(BonesAuthenticationStateProvider))));

        return authenticationState;
    }

    public async Task SetCurrentUserAsync(object? currentUser, CancellationToken cancellationToken)
    {
        await localStorageService.SetItemAsync(LocalStorageService.CURRENT_USER_KEY, currentUser, cancellationToken);

        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public Task<object?> GetCurrentUserAsync(CancellationToken cancellationToken)
    {
        return localStorageService.GetItemAsync<object>(LocalStorageService.CURRENT_USER_KEY, cancellationToken);
    }
}