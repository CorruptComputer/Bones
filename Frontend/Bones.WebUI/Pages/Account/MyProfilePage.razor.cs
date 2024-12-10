using System.Globalization;
using Bones.Api.Client;
using Bones.Shared.Consts;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Bones.WebUI.Pages.Account;

/// <summary>
///   The user can view and update their profile here
/// </summary>
public partial class MyProfilePage : ComponentBase
{
    private bool ProfileUpdateSuccess { get; set; } = false;

    /// <summary>
    ///   Is the form valid?
    /// </summary>
    public bool FormValid { get; set; }

    /// <summary>
    ///   The issues with the users input
    /// </summary>
    public string[] ValidationErrors { get; set; } = [];

    private MudTextField<string> Email { get; set; } = new();

    private MudTextField<string> EmailConfirmed { get; set; } = new();

    private MudTextField<string> EmailConfirmedDateTime { get; set; } = new();

    private MudTextField<string> DisplayName { get; set; } = new();

    private MudTextField<string> CreateDateTime { get; set; } = new();

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        GetMyProfileResponse response = await ApiClient.GetMyProfileAsync();

        await CreateDateTime.SetText(response.CreateDateTime.LocalDateTime.ToString(CultureInfo.CurrentCulture) ?? string.Empty);

        await Email.SetText(response.Email ?? string.Empty);
        await EmailConfirmed.SetText(response.EmailConfirmed.ToString() ?? string.Empty);
        await EmailConfirmedDateTime.SetText(response.EmailConfirmedDateTime?.LocalDateTime.ToString(CultureInfo.CurrentCulture) ?? string.Empty);

        await DisplayName.SetText(response.DisplayName ?? string.Empty);

        await base.OnInitializedAsync();
    }

    /// <summary>
    ///   Updates the users profile with the info provided
    /// </summary>
    public async Task UpdateProfileAsync()
    {
        string? displayName = DisplayName.Text;

        if (string.IsNullOrWhiteSpace(displayName))
        {
            return;
        }

        // TODO: Api 
        await Task.Run(() => Thread.Sleep(1));
    }

    /// <summary>
    ///   Redirects the user to the change email page
    /// </summary>
    public void GoToChangeEmail()
    {
        NavManager.NavigateTo(FrontEndUrls.Account.CHANGE_EMAIL);
    }

    /// <summary>
    ///   Redirects the user to the change password page
    /// </summary>
    public void GoToChangePassword()
    {
        NavManager.NavigateTo(FrontEndUrls.Account.CHANGE_PASSWORD);
    }
}