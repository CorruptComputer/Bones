using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Bones.WebUI.Pages.Auth;

public partial class Login
{
    [Parameter]
    public string? ReturnUrl { get; set; }

    public bool FormValid { get; set; }

    public string[] ValidationErrors { get; set; } = [];

    private MudTextField<string> EmailAddress { get; set; } = new();

    private MudTextField<string> Password { get; set; } = new();

    private bool ErrorLoggingIn { get; set; } = false;

    public Task DoLoginAsync()
    {
        ErrorLoggingIn = false;

        try
        {
            //await ApiClient.Login(true, false, new()
            //{
            //    Email = EmailAddress.Text,
            //    Password = Password.Text
            //});

            //// Now refresh the Authentication State:
            //// Ideally we would want to change this to something custom, but it'll work for now
            //ApiResult<InfoResponse> me = await ApiClient.AuthManageInfoGetAsync();
            //await AuthStateProvider.SetCurrentUserAsync(me.Result);

            NavManager.NavigateTo(GetNavigationUrl());
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error while registering user");
            ErrorLoggingIn = true;
        }

        return Task.CompletedTask;
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