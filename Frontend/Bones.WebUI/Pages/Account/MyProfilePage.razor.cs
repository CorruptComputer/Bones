using System.Globalization;

namespace Bones.WebUI.Pages.Account;

/// <summary>
///   The user can view and update their profile here
/// </summary>
/// <param name="apiClient"></param>
/// <param name="navManager"></param>
/// <param name="logger"></param>
public partial class MyProfilePage(BonesApiClient apiClient, NavigationManager navManager, ILogger<MyProfilePage> logger) : ComponentBase
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

    private bool AccountAuditsLoading { get; set; } = true;

    private IOrderedEnumerable<MyAccountAuditModel>? AccountAudits { get; set; }

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
        AccountAuditsLoading = true;

        GetMyProfileResponse? response = await apiClient.Account.My.Profile.GetAsync();

        if (response is null)
        {
            logger.LogError("Failed to get user profile from API");
            return;
        }

        CreateDateTime = response.CreateDateTime.LocalDateTime.ToString(CultureInfo.CurrentCulture);
        PasswordLastSet = response.PasswordLastSetDateTime.LocalDateTime.ToString(CultureInfo.CurrentCulture);
        Email = response.Email;
        EmailConfirmed = response.EmailConfirmed ? response.EmailConfirmedDateTime?.LocalDateTime.ToString(CultureInfo.CurrentCulture) ?? "Not confirmed" : "Not confirmed";
        DisplayName = response.DisplayName;
        AccountAudits = response.AccountAudits.OrderByDescending(x => x.DateTime);
        AccountAuditsLoading = false;
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

        await apiClient.Account.My.Profile.PutAsync(new UpdateMyProfileRequest
        {
            DisplayName = DisplayName
        });

        await FetchFromApi();
    }

    /// <summary>
    ///   Redirects the user to the change email page
    /// </summary>
    public void GoToChangeEmail()
    {
        navManager.NavigateTo(FrontEndUrls.Account.CHANGE_EMAIL);
    }

    /// <summary>
    ///   Redirects the user to the change password page
    /// </summary>
    public void GoToChangePassword()
    {
        navManager.NavigateTo(FrontEndUrls.Account.CHANGE_PASSWORD);
    }
}