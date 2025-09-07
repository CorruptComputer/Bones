using System.ComponentModel.DataAnnotations;

namespace Bones.WebUI.Components.SystemAdmin.SystemSettings;

/// <summary>
///   Component for viewing and updating the background service user configuration
/// </summary>
/// <param name="apiClient"></param>
public partial class BackgroundServiceUserConfigurationComponent(BonesApiClient apiClient) : ComponentBase
{
    private bool ApiError { get; set; } = false;

    private BackgroundServiceUserConfigFormModel Model { get; set; } = new();

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        await FetchFromAPI();

        await base.OnInitializedAsync();
    }

    /// <inheritdoc />
    protected override async Task OnParametersSetAsync()
    {
        await FetchFromAPI();

        await base.OnParametersSetAsync();
    }

    private async Task FetchFromAPI()
    {
        ApiError = false;

        try
        {
            GetBackgroundServiceUserConfigResponse? backgroundServiceUserConfig = await apiClient.SysAdmin.Settings.BackgroundServiceUserConfig.GetAsync();
            if (backgroundServiceUserConfig is null)
            {
                ApiError = true;
                return;
            }

            Model.Email = backgroundServiceUserConfig.Email;
            Model.DisplayName = backgroundServiceUserConfig.DisplayName;
        }
        catch
        {
            ApiError = true;
        }
    }

    private async Task Update()
    {
        ApiError = false;
        try
        {
            await apiClient.SysAdmin.Settings.BackgroundServiceUserConfig.PostAsync(new()
            {
                Email = Model.Email,
                DisplayName = Model.DisplayName,
                ChangeReason = Model.ChangeReason
            });
        }
        catch
        {
            ApiError = true;
        }
    }

    private sealed class BackgroundServiceUserConfigFormModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string DisplayName { get; set; } = string.Empty;

        [Required]
        public string ChangeReason { get; set; } = string.Empty;
    }
}
