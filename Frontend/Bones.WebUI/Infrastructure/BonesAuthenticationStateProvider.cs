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
        GetMyProfileResponse? currentUser = await GetCurrentUserAsync(CancellationToken.None);

        if (currentUser == null)
        {
            return new(new());
        }

        List<Claim> claims = [
            new(BonesClaimTypes.User.EMAIL, currentUser.Email),
            new(BonesClaimTypes.User.DISPLAY_NAME, currentUser.DisplayName)
        ];

        if (currentUser.IsSysAdmin)
        {
            claims.Add(new(BonesClaimTypes.Role.System.SYSTEM_ADMINISTRATOR, ClaimValues.YES));
        }

        return new(new(new ClaimsIdentity(claims, authenticationType: nameof(BonesAuthenticationStateProvider))));
    }

    /// <summary>
    ///   Sets the current user
    /// </summary>
    /// <param name="currentUser"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task SetCurrentUserAsync(GetMyProfileResponse currentUser, CancellationToken cancellationToken)
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
    public Task<GetMyProfileResponse?> GetCurrentUserAsync(CancellationToken cancellationToken)
    {
        return localStorageService.GetItemAsync<GetMyProfileResponse>(LocalStorageService.CURRENT_USER_KEY, cancellationToken);
    }
}