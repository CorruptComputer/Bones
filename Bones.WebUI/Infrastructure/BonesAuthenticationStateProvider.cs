using System.Security.Claims;
using Bones.WebUI.ApiClients;
using Microsoft.AspNetCore.Components.Authorization;

namespace Bones.WebUI.Infrastructure;

public class BonesAuthenticationStateProvider(LocalStorageService localStorageService) : AuthenticationStateProvider
{
    

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        InfoResponse? currentUser = await GetCurrentUserAsync();

        if(currentUser == null)
        {
            return new(new(new ClaimsIdentity()));
        }

        Claim[] claims = [
            new(ClaimTypes.Email, currentUser.Email)
        ];

        AuthenticationState? authenticationState = new(new(new ClaimsIdentity(claims, authenticationType: nameof(BonesAuthenticationStateProvider))));

        return authenticationState;
    }

    public async Task SetCurrentUserAsync(InfoResponse? currentUser)
    { 
        await localStorageService.SetItem(LocalStorageService.CURRENT_USER_KEY, currentUser);

        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public Task<InfoResponse?> GetCurrentUserAsync() => localStorageService.GetItemAsync<InfoResponse>(LocalStorageService.CURRENT_USER_KEY);
}