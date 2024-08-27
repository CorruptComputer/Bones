using Bones.Api.Client;

namespace Bones.WebUI.Pages.Account;

public partial class MyProfilePage
{
    private string Email { get; set; } = string.Empty;
    private string DisplayName { get; set; } = string.Empty;


    protected override async Task OnInitializedAsync()
    {
        GetMyBasicInfoResponseActionResult response = await ApiClient.GetMyBasicInfoAsync();

        Email = response.Value.Email ?? string.Empty;
        DisplayName = response.Value.DisplayName ?? string.Empty;

        await base.OnInitializedAsync();
    }
}