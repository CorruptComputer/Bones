namespace Bones.WebUI.Pages.Auth;

public partial class MyProfile
{
    private string Email { get; set; } = string.Empty;
    private bool EmailConfirmed { get; set; } = false;


    protected override async Task OnInitializedAsync()
    {
        //ApiResult<InfoResponse> response = await ApiClient.AuthManageInfoGetAsync();
        //Email = response.Result.Email;
        //EmailConfirmed = response.Result.IsEmailConfirmed;

        await base.OnInitializedAsync();
    }
}