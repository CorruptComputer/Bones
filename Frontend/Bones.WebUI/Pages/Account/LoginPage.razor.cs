using Bones.Api.Client;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Bones.WebUI.Pages.Account;

public partial class LoginPage
{
    [Parameter]
    public string? ReturnUrl { get; set; }

    public bool FormValid { get; set; }

    public string[] ValidationErrors { get; set; } = [];

    private MudTextField<string> EmailAddress { get; set; } = new();

    private MudTextField<string> Password { get; set; } = new();

    private bool ErrorLoggingIn { get; set; } = false;

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
            GetMyBasicInfoResponseActionResult me = await ApiClient.GetMyBasicInfoAsync();
            await AuthStateProvider.SetCurrentUserAsync(me.Value, CancellationToken.None);

            NavManager.NavigateTo(GetNavigationUrl());
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error while logging in");
            ErrorLoggingIn = true;
        }
    }

    private string GetNavigationUrl()
    {
        if (string.IsNullOrWhiteSpace(ReturnUrl) || !IsSafeRedirect(ReturnUrl))
        {
            return "/";
        }

        return ReturnUrl;
    }

    /// <summary>
    ///   We only want to redirect them within our own application, don't want to redirect to anywhere else.
    /// </summary>
    /// <param name="uri"></param>
    /// <returns></returns>
    private bool IsSafeRedirect(string uri)
    {
        if (Uri.IsWellFormedUriString(uri, UriKind.Absolute))
        {
            return false;
        }

        Uri parsedUri = new(uri, UriKind.RelativeOrAbsolute);
        if (parsedUri.IsAbsoluteUri || !string.IsNullOrEmpty(parsedUri.Host))
        {
            return false;
        }

        return true;
    }







}