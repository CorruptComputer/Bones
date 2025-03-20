using System.ComponentModel.DataAnnotations;

namespace Bones.WebUI.Components.SystemAdmin.SystemSettings;

/// <summary>
///   Component for viewing and updating the web UI base URL
/// </summary>
/// <param name="apiClient"></param>
public partial class WebUiBaseUrlComponent(BonesApiClient apiClient) : ComponentBase
{
    private bool ApiError { get; set; } = false;

    private WebUiBaseUrlFormModel Model { get; set; } = new();

    

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
        ApiError = false;

        try
        {
            string baseUrl = await apiClient.GetWebUiBaseUrlAsync();
            Model = new()
            {
                BaseUrl = baseUrl
            };
        }
        catch
        {
            ApiError = true;
        }
    }

    /// <summary>
    ///   Sends the request to update the web UI base URL
    /// </summary>
    /// <returns></returns>
    protected async Task Update()
    {
        ApiError = false;

        try
        {
            await apiClient.SaveWebUiBaseUrlAsync(new()
            {
                BaseUrl = Model.BaseUrl,
                ChangeReason = Model.ChangeReason
            });
        }
        catch
        {
            ApiError = true;
        }
    }

    private class WebUiBaseUrlFormModel
    {
        [Required(ErrorMessage = "Web UI Base URL is required!")]
        public string? BaseUrl { get; set; }

        [Required(ErrorMessage = "Change reason is required!")]
        public string? ChangeReason { get; set; }
    }
}
