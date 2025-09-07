using System.Net;
using ReQuesty.Runtime.Abstractions;

namespace Bones.WebUI.Pages.Asset;

/// <summary>
///   Page for viewing an asset
/// </summary>
public partial class ViewAssetPage(BonesApiClient apiClient, NavigationManager navManager, ILogger<ViewAssetPage> logger) : ComponentBase
{
    /// <summary>
    ///   The ID of the asset to view
    /// </summary>
    [Parameter]
    public required Guid AssetId { get; set; }

    /// <summary>
    ///   Did the request to the API result in an error?
    /// </summary>
    protected bool ApiError { get; set; } = false;

    private int _currentVersion = 0;
    private Guid _projectId = Guid.Empty;
    private string _assetTitle { get; set; } = string.Empty;

    /// <summary>
    ///   Fires when the page is loaded
    /// </summary>
    /// <returns></returns>
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
            GetAssetByIdResponse? assetResponse = await apiClient.Asset[AssetId].GetAsync();

            if (assetResponse is not null)
            {
                _projectId = assetResponse.ProjectId;
                _currentVersion = assetResponse.CurrentVersion;
                _assetTitle = assetResponse.Title;
            }
        }
        catch (ApiException ex) when (ex.ResponseStatusCode == (int)HttpStatusCode.NotFound)
        {
            logger.LogWarning("Asset with ID {AssetId} not found, redirecting to home", AssetId);
            navManager.NavigateTo("/");
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error fetching asset with ID {AssetId}", AssetId);
            ApiError = true;
        }
    }
}
