namespace Bones.WebUI.Components.SystemAdmin.SystemSettings;

/// <summary>
///   Component for viewing and updating the system admin mask user
/// </summary>
/// <param name="apiClient"></param>
public partial class SystemAdminMaskUserConfigComponent(BonesApiClient apiClient) : ComponentBase
{
    /// <summary>
    ///   Is the form valid?
    /// </summary>
    protected bool FormValid { get; set; }

    /// <summary>
    ///   The issues with the users input
    /// </summary>
    protected string[] ValidationErrors { get; set; } = [];

    /// <summary>
    ///   Is the mask user enabled?
    /// </summary>
    public bool? IsEnabled { get; set; }

    /// <summary>
    ///   The email of the mask user
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    ///   The display name of the mask user
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    ///   If updating, the reason for the change
    /// </summary>
    public string? ChangeReason { get; set; }

    /// <summary>
    ///   Fires when the page is loaded
    /// </summary>
    protected override async Task OnInitializedAsync()
    {
        await FetchFromAPI();

        await base.OnInitializedAsync();
    }

    /// <summary>
    ///   Fires if the same page but with a different parameter is loaded
    /// </summary>
    /// <returns></returns>
    protected override async Task OnParametersSetAsync()
    {
        await FetchFromAPI();

        await base.OnParametersSetAsync();
    }

    private async Task FetchFromAPI()
    {
        GetSystemAdminMaskUserConfigResponse maskUserConfig = await apiClient.GetSystemAdminMaskUserConfigAsync();

        IsEnabled = maskUserConfig.IsEnabled;
        Email = maskUserConfig.Email;
        DisplayName = maskUserConfig.DisplayName;
    }

    /// <summary>
    ///   Sends the request to update the background service user configuration
    /// </summary>
    /// <returns></returns>
    protected async Task Update()
    {
        if (!FormValid)
        {
            return;
        }

        try
        {
            await apiClient.SaveSystemAdminMaskUserConfigAsync(new()
            {
                IsEnabled = IsEnabled ?? false,
                Email = Email,
                DisplayName = DisplayName,
                ChangeReason = ChangeReason
            });
        }
        catch (ApiException e)
        {
            ValidationErrors = [e.Message];
        }
    }
}
