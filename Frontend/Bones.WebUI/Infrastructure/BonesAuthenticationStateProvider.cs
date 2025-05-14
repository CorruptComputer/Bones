using System.Security.Claims;
using Bones.Shared.Consts;
using Microsoft.AspNetCore.Components.Authorization;

namespace Bones.WebUI.Infrastructure;

/// <summary>
///   Provides the state of authentication
/// </summary>
/// <param name="localStorageService"></param>
/// <param name="sessionStorageService"></param>
/// <param name="logger"></param>
/// <param name="serviceProvider"></param>
public class BonesAuthenticationStateProvider(LocalStorageService localStorageService, SessionStorageService sessionStorageService, ILogger<BonesAuthenticationStateProvider> logger, IServiceProvider serviceProvider) : AuthenticationStateProvider
{
    /// <inheritdoc />
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        string? base64LocalStorageKey = await sessionStorageService.GetItemAsync<string>(SessionStorageService.BASE64_LOCALSTORAGE_KEY, CancellationToken.None);

        if (base64LocalStorageKey == null)
        {
            Guid? sessionId = await localStorageService.GetItemAsync<Guid?>(LocalStorageService.SESSION_ID_KEY, string.Empty, CancellationToken.None);
            if (sessionId == null)
            {
                logger.LogInformation("No session ID found in local storage");
                return new(new());
            }

            // This class is Singleton scoped, so we need to create a new scope for every request to get the ApiClient.
            using IServiceScope scope = serviceProvider.CreateScope();
            BonesApiClient client = scope.ServiceProvider.GetRequiredService<BonesApiClient>();
            GetOrCreateMySessionResponse session = await client.GetOrCreateMySessionAsync(sessionId.Value, CancellationToken.None);

            if (session == null)
            {
                logger.LogWarning("Session not found or invalidated server-side, clearing local storage");
                await localStorageService.ClearAsync(CancellationToken.None);

                return new(new());
            }

            base64LocalStorageKey = session.Base64LocalStorageKey;
            await sessionStorageService.SetItemAsync(SessionStorageService.BASE64_LOCALSTORAGE_KEY, base64LocalStorageKey, CancellationToken.None);
        }

        GetMyProfileResponse? currentUser = await GetCurrentUserFromBrowserStorageAsync(base64LocalStorageKey, CancellationToken.None);

        if (currentUser == null)
        {
            logger.LogWarning("No current user found in local storage, clearing local storage");
            await localStorageService.ClearAsync(CancellationToken.None);
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
    /// <param name="SessionId"></param>
    /// <param name="base64LocalStorageKey"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task SaveCurrentUserInBrowserStorageAsync(GetMyProfileResponse currentUser, Guid SessionId, string base64LocalStorageKey, CancellationToken cancellationToken)
    {
        logger.LogInformation("Saving current user to browser storage");
        bool currentUserSuccess = await localStorageService.SetItemAsync(LocalStorageService.CURRENT_USER_KEY, currentUser, base64LocalStorageKey, cancellationToken);
        bool sessionIdSuccess = await localStorageService.SetItemAsync(LocalStorageService.SESSION_ID_KEY, SessionId, base64LocalStorageKey, cancellationToken);
        await sessionStorageService.SetItemAsync(SessionStorageService.BASE64_LOCALSTORAGE_KEY, base64LocalStorageKey, cancellationToken);

        if (!currentUserSuccess || !sessionIdSuccess)
        {
            logger.LogWarning("Failed to save current user to browser storage");
            await localStorageService.ClearAsync(cancellationToken);
        }

        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    /// <summary>
    ///   Clears the current user
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task ClearCurrentUserInBrowserStorageAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Clearing current user from browser storage");
        await localStorageService.RemoveItemAsync(LocalStorageService.CURRENT_USER_KEY, cancellationToken);
        await localStorageService.RemoveItemAsync(LocalStorageService.SESSION_ID_KEY, cancellationToken);

        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    /// <summary>
    ///   Gets the current user
    /// </summary>
    /// <param name="base64LocalStorageKey"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<GetMyProfileResponse?> GetCurrentUserFromBrowserStorageAsync(string base64LocalStorageKey, CancellationToken cancellationToken)
    {
        return localStorageService.GetItemAsync<GetMyProfileResponse>(LocalStorageService.CURRENT_USER_KEY, base64LocalStorageKey, cancellationToken);
    }
}