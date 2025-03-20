using System.ComponentModel.DataAnnotations;

namespace Bones.WebUI.Components.SystemAdmin.SystemSettings;

/// <summary>
///   Component for viewing and updating the system admin mask user
/// </summary>
/// <param name="apiClient"></param>
public partial class SystemAdminMaskUserConfigComponent(BonesApiClient apiClient) : ComponentBase
{
    private bool ApiError { get; set; } = false;

    private SystemAdminMaskUserFormModel Model { get; set; } = new();

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
            GetSystemAdminMaskUserConfigResponse maskUserConfig = await apiClient.GetSystemAdminMaskUserConfigAsync();

            Model = new()
            {
                IsEnabled = maskUserConfig.IsEnabled,
                Email = maskUserConfig.Email,
                DisplayName = maskUserConfig.DisplayName
            };
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
            await apiClient.SaveSystemAdminMaskUserConfigAsync(new()
            {
                IsEnabled = Model.IsEnabled,
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

    private class SystemAdminMaskUserFormModel
    {
        public bool IsEnabled { get; set; }

        [Required(ErrorMessage = "Email is required!")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Display Name is required!")]
        public string? DisplayName { get; set; }

        [Required(ErrorMessage = "Change reason is required!")]
        public string? ChangeReason { get; set; }
    }
}
