using System.Net;
using System.Security.Claims;
using Bones.Shared.Consts;
using Bones.WebUI.Consts;
using Microsoft.AspNetCore.Components.Authorization;
using ReQuesty.Runtime.Abstractions;

namespace Bones.WebUI.Services.Singleton;

/// <summary>
///   Provides the state of authentication
/// </summary>
/// <param name="localStorageService"></param>
/// <param name="sessionStorageService"></param>
/// <param name="logger"></param>
/// <param name="serviceProvider"></param>
public class BonesAuthenticationStateProvider(LocalStorageService localStorageService, SessionStorageService sessionStorageService,
                                              ILogger<BonesAuthenticationStateProvider> logger, IServiceProvider serviceProvider) : AuthenticationStateProvider
{
    // This class is Singleton scoped, so we need to create a new scope for every request to get the ApiClient.
    // Ideally each method should really only need to use this once, but the performance hit for multiple should be negligible
    private BonesApiClient ApiClient => serviceProvider.CreateScope().ServiceProvider.GetRequiredService<BonesApiClient>();

    /// <inheritdoc />
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        string? base64LocalStorageKey = await sessionStorageService.GetItemAsync<string>(SessionStorageConsts.BASE64_LOCALSTORAGE_KEY, CancellationToken.None);

        if (base64LocalStorageKey == null)
        {
            Guid? sessionId = await localStorageService.GetItemAsync<Guid?>(LocalStorageConsts.SESSION_ID_KEY, string.Empty, CancellationToken.None);
            if (sessionId == null)
            {
                logger.LogInformation("No session ID found in local storage");
                return new(new());
            }

            try
            {
                GetOrCreateMySessionResponse? session = await ApiClient.Account.My.Session.GetAsync(req => req.QueryParameters.SessionId = sessionId.Value.ToString());
                if (session is null)
                {
                    throw new InvalidOperationException("Received null session from API");
                }

                base64LocalStorageKey = session.Base64LocalStorageKey;
                await sessionStorageService.SetItemAsync(SessionStorageConsts.BASE64_LOCALSTORAGE_KEY, base64LocalStorageKey, CancellationToken.None);
            }
            // 404 is a definite sign the session invalid
            catch (ApiException ex) when (ex.ResponseStatusCode == (int)HttpStatusCode.NotFound)
            {
                logger.LogWarning("Session not found or invalidated server-side, clearing local storage");
                await ClearCurrentUserInBrowserStorageAsync(CancellationToken.None);

                return new(new());
            }
            // 401 means the auth token is invalid, which means the session will be gone too
            catch (ApiException ex) when (ex.ResponseStatusCode == (int)HttpStatusCode.Unauthorized)
            {
                logger.LogWarning("Token is invalid, clearing local storage");
                await ClearCurrentUserInBrowserStorageAsync(CancellationToken.None);

                return new(new());
            }
            // anything else could be whatever, who knows. Not the best way to handle this
            // ideally it would check if there is internet connectivity and retry when it connects
            // PWAs can be installed for offline use, so I'd like to have that be possible since it was the entire point of encrypting localstorage
            // but thats a later problem
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting session from server, clearing local storage to prevent getting stuck in a broken state");
                await ClearCurrentUserInBrowserStorageAsync(CancellationToken.None);

                return new(new());
            }
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
        bool currentUserSuccess = await localStorageService.SetItemAsync(LocalStorageConsts.CURRENT_USER_KEY, currentUser, base64LocalStorageKey, cancellationToken);
        bool sessionIdSuccess = await localStorageService.SetItemAsync(LocalStorageConsts.SESSION_ID_KEY, SessionId, base64LocalStorageKey, cancellationToken);
        await sessionStorageService.SetItemAsync(SessionStorageConsts.BASE64_LOCALSTORAGE_KEY, base64LocalStorageKey, cancellationToken);

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
        await ApiClient.Login.Logout.PostAsync(cancellationToken: cancellationToken);
        await localStorageService.RemoveItemAsync(LocalStorageConsts.CURRENT_USER_KEY, cancellationToken);
        await localStorageService.RemoveItemAsync(LocalStorageConsts.SESSION_ID_KEY, cancellationToken);
        await sessionStorageService.RemoveItemAsync(SessionStorageConsts.BASE64_LOCALSTORAGE_KEY, CancellationToken.None);

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
        return localStorageService.GetItemAsync<GetMyProfileResponse>(LocalStorageConsts.CURRENT_USER_KEY, base64LocalStorageKey, cancellationToken);
    }
}