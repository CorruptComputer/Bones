namespace Bones.WebUI.Pages.Asset;

/// <summary>
///   Page for viewing an asset layouts dashboard
/// </summary>
/// <param name="apiClient"></param>
/// <param name="logger"></param>
public partial class AssetLayoutDashboardPage(BonesApiClient apiClient, ILogger<AssetLayoutDashboardPage> logger) : ComponentBase
{
    /// <summary>
    ///   The ID of the Asset Layout to load in this dashboard
    /// </summary>
    [Parameter]
    public Guid AssetLayoutId { get; set; }

    /// <summary>
    ///   The name of the AssetLayout, received from the API
    /// </summary>
    public string? LayoutName { get; set; }

    /// <summary>
    ///   The assets in this layout, ordered by date added from oldest to newest
    /// </summary>
    public IOrderedEnumerable<DashboardAssetModel>? BonesAssets { get; set; }

    private bool AssetsLoading { get; set; } = true;

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
        AssetsLoading = true;
        GetAssetLayoutDashboardResponse? dashboardResponse = await apiClient.Asset.Layout[AssetLayoutId].Dashboard.GetAsync();
        if (dashboardResponse is null)
        {
            logger.LogError("Failed to get asset layout dashboard for layout ID {AssetLayoutId}", AssetLayoutId);
            return;
        }

        LayoutName = dashboardResponse.LayoutName;
        BonesAssets = dashboardResponse.Assets.OrderBy(x => x.LatestVersionCreateDateTime);
        AssetsLoading = false;
    }
}
