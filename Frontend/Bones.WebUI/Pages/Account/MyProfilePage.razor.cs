using System.Globalization;
using Bones.Api.Client;
using Bones.Shared.Consts;
using MudBlazor;

namespace Bones.WebUI.Pages.Account;

public partial class MyProfilePage
{
    private bool ProfileUpdateSuccess { get; set; } = false;
    public bool FormValid { get; set; }

    public string[] ValidationErrors { get; set; } = [];

    private MudTextField<string> Email { get; set; } = new();

    private MudTextField<string> EmailConfirmed { get; set; } = new();

    private MudTextField<string> EmailConfirmedDateTime { get; set; } = new();

    private MudTextField<string> DisplayName { get; set; } = new();

    private MudTextField<string> CreateDateTime { get; set; } = new();


    protected override async Task OnInitializedAsync()
    {
        GetMyProfileResponse response = await ApiClient.GetMyProfileAsync();

        await CreateDateTime.SetText(response.CreateDateTime?.LocalDateTime.ToString(CultureInfo.CurrentCulture) ?? string.Empty);

        await Email.SetText(response.Email ?? string.Empty);
        await EmailConfirmed.SetText(response.EmailConfirmed?.ToString() ?? string.Empty);
        await EmailConfirmedDateTime.SetText(response.EmailConfirmedDateTime?.LocalDateTime.ToString(CultureInfo.CurrentCulture) ?? string.Empty);

        await DisplayName.SetText(response.DisplayName ?? string.Empty);

        await base.OnInitializedAsync();
    }

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

    public void GoToChangeEmail()
    {
        NavManager.NavigateTo(FrontEndUrls.Account.CHANGE_EMAIL);
    }

    public void GoToChangePassword()
    {
        NavManager.NavigateTo(FrontEndUrls.Account.CHANGE_PASSWORD);
    }
}