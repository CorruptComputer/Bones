using Bones.WebUI.Infrastructure;
using MudBlazor;

namespace Bones.WebUI.Components.Auth;

/// <summary>
///   Page to login
/// </summary>
/// <param name="apiClient"></param>
/// <param name="authStateProvider"></param>
/// <param name="logger"></param>
public partial class LoginComponent(BonesApiClient apiClient, BonesAuthenticationStateProvider authStateProvider, ILogger<LoginComponent> logger) : ComponentBase
{
    /// <summary>
    ///   Is the form valid?
    /// </summary>
    public bool FormValid { get; set; }

    /// <summary>
    ///   The issues with the users input
    /// </summary>
    public string[] ValidationErrors { get; set; } = [];

    private MudTextField<string> EmailAddress { get; set; } = new();

    private MudTextField<string> Password { get; set; } = new();

    private bool ErrorLoggingIn { get; set; } = false;

    /// <summary>
    ///   Sends the request to login, checks that it was successful, and redirects them somewhere else.
    /// </summary>
    public async Task DoLoginAsync()
    {
        ErrorLoggingIn = false;

        try
        {
            // We won't get anything useful back in the response, instead the browser will be told to save the login as a cookie with the headers
            // if this fails it'll throw an exception
            await apiClient.LoginAsync(new()
            {
                Email = EmailAddress.Text,
                Password = Password.Text
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
}