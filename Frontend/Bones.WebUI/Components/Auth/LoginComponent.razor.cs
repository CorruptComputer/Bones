using System.ComponentModel.DataAnnotations;
using Bones.WebUI.Infrastructure;

namespace Bones.WebUI.Components.Auth;

/// <summary>
///   Page to login
/// </summary>
/// <param name="apiClient"></param>
/// <param name="authStateProvider"></param>
/// <param name="logger"></param>
public partial class LoginComponent(BonesApiClient apiClient, BonesAuthenticationStateProvider authStateProvider, ILogger<LoginComponent> logger) : ComponentBase
{
    private LoginFormModel LoginForm { get; set; } = new();

    private bool ErrorLoggingIn { get; set; } = false;

    private async Task DoLoginAsync()
    {
        ErrorLoggingIn = false;

        try
        {
            // We won't get anything useful back in the response, instead the browser will be told to save the login as a cookie with the headers
            // if this fails it'll throw an exception
            await apiClient.LoginAsync(new()
            {
                Email = LoginForm.Email,
                Password = LoginForm.Password
            });

            // Now refresh the Authentication State:
            GetMyProfileResponse? me = await apiClient.GetMyProfileAsync();
            if (me == null)
            {
                logger.LogError("Error getting my profile after logging in");
                ErrorLoggingIn = true;
                return;
            }

            GetOrCreateMySessionResponse session = await apiClient.GetOrCreateMySessionAsync(null, CancellationToken.None);

            await authStateProvider.SaveCurrentUserInBrowserStorageAsync(me, session.SessionId, session.Base64LocalStorageKey, CancellationToken.None);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error logging in");
            ErrorLoggingIn = true;
        }
    }

    private sealed class LoginFormModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(8)]
        public string Password { get; set; } = string.Empty;
    }
}