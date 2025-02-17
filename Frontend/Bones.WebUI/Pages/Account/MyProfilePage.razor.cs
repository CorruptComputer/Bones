using System.Globalization;
using Bones.Api.Client;
using Bones.Shared.Consts;
using Microsoft.AspNetCore.Components;

namespace Bones.WebUI.Pages.Account;

/// <summary>
///   The user can view and update their profile here
/// </summary>
public partial class MyProfilePage(BonesApiClient ApiClient, NavigationManager NavManager) : ComponentBase
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

    private string CreateDateTime { get; set; } = string.Empty;

    private string PasswordLastSet { get; set; } = string.Empty;

    private string Email { get; set; } = string.Empty;

    private string EmailConfirmed { get; set; } = string.Empty;

    private string DisplayName { get; set; } = string.Empty;

    /// <summary>
    ///   Event for when the page is loaded
    /// </summary>
    protected override async Task OnInitializedAsync()
    {
        await FetchFromApi();

        await base.OnInitializedAsync();
    }

    /// <summary>
    ///   Event for when the page is changed without a full site reload
    /// </summary>
    /// <returns></returns>
    protected override async Task OnParametersSetAsync()
    {
        await FetchFromApi();

        await base.OnParametersSetAsync();
    }

    private async Task FetchFromApi()
    {
        GetMyProfileResponse response = await ApiClient.GetMyProfileAsync();

        CreateDateTime = response.CreateDateTime.LocalDateTime.ToString(CultureInfo.CurrentCulture);
        PasswordLastSet = response.PasswordLastSetDateTime.LocalDateTime.ToString(CultureInfo.CurrentCulture);
        Email = response.Email;
        EmailConfirmed = response.EmailConfirmed ? response.EmailConfirmedDateTime?.LocalDateTime.ToString(CultureInfo.CurrentCulture) ?? "Not confirmed" : "Not confirmed";
        DisplayName = response.DisplayName;
    }

    /// <summary>
    ///   Updates the users profile with the info provided
    /// </summary>
    public async Task UpdateProfileAsync()
    {
        if (string.IsNullOrWhiteSpace(DisplayName))
        {
            return;
        }

        await ApiClient.UpdateMyProfileAsync(new UpdateMyProfileRequest
        {
            DisplayName = DisplayName
        });
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