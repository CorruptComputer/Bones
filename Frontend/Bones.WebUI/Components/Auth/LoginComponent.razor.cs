using Bones.Api.Client;
using Bones.WebUI.Infrastructure;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Bones.WebUI.Components.Auth;

/// <summary>
///   Page to login
/// </summary>
public partial class LoginComponent(BonesApiClient ApiClient, BonesAuthenticationStateProvider AuthStateProvider) : ComponentBase
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
            await ApiClient.LoginAsync(new()
            {
                Email = EmailAddress.Text,
                Password = Password.Text
            });

            // Now refresh the Authentication State:
            GetMyProfileResponse? me = await ApiClient.GetMyProfileAsync();
            if (me == null)
            {
                ErrorLoggingIn = true;
                return;
            }

            await AuthStateProvider.SaveCurrentUserInBrowserStorageAsync(me, CancellationToken.None);
        }
        catch
        {
            ErrorLoggingIn = true;
        }
    }
}