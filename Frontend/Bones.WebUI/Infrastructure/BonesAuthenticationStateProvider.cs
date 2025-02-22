using System.Security.Claims;
using Bones.Shared.Consts;
using Microsoft.AspNetCore.Components.Authorization;

namespace Bones.WebUI.Infrastructure;

/// <summary>
///   Provides the state of authentication
/// </summary>
/// <param name="localStorageService"></param>
/// <param name="logger"></param>
public class BonesAuthenticationStateProvider(LocalStorageService localStorageService, ILogger<BonesAuthenticationStateProvider> logger) : AuthenticationStateProvider
{
    /// <inheritdoc />
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        GetMyProfileResponse? currentUser = await GetCurrentUserFromBrowserStorageAsync(CancellationToken.None);

        if (currentUser == null)
        {
            return new(new());
        }

        List<Claim> claims = [
            new(BonesClaimTypes.User.EMAIL, currentUser.Email),
            new(BonesClaimTypes.User.DISPLAY_NAME, currentUser.DisplayName)
        ];

        // SysAdmin stuff is dynamically hidden from the UI, organizational roles are handled server side 
        if (currentUser.IsSysAdmin)
        {
            logger.LogInformation("User is a system administrator");
            claims.Add(new(ClaimsIdentity.DefaultRoleClaimType, SystemRoles.SYSTEM_ADMINISTRATORS));
        }

        ClaimsIdentity identity = new(claims, authenticationType: nameof(BonesAuthenticationStateProvider));
        ClaimsPrincipal principal = new(identity);

        return new(principal);
    }

    /// <summary>
    ///   Saves the current user
    /// </summary>
    /// <param name="currentUser"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task SaveCurrentUserInBrowserStorageAsync(GetMyProfileResponse currentUser, CancellationToken cancellationToken)
    {
        await localStorageService.SetItemAsync(LocalStorageService.CURRENT_USER_KEY, currentUser, cancellationToken);

        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    /// <summary>
    ///   Clears the current user
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task ClearCurrentUserInBrowserStorageAsync(CancellationToken cancellationToken)
    {
        await localStorageService.RemoveItemAsync(LocalStorageService.CURRENT_USER_KEY, cancellationToken);

        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    /// <summary>
    ///   Gets the current user
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<GetMyProfileResponse?> GetCurrentUserFromBrowserStorageAsync(CancellationToken cancellationToken)
    {
        return localStorageService.GetItemAsync<GetMyProfileResponse>(LocalStorageService.CURRENT_USER_KEY, cancellationToken);
    }
}