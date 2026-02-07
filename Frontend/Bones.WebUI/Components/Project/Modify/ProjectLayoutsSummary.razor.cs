namespace Bones.WebUI.Components.Project.Modify;

/// <summary>
///   Component for viewing a project's item layouts
/// </summary>
public partial class ProjectLayoutsSummary(BonesApiClient apiClient, ILogger<ProjectLayoutsSummary> logger) : ComponentBase
{
    /// <summary>
    ///   The ID of the project to load
    /// </summary>
    [Parameter, EditorRequired]
    public required Guid ProjectId { get; set; }

    /// <summary>
    ///   The number of item layouts on the project, received from the API
    /// </summary>
    public int? ItemLayoutsCount { get; set; }

    /// <summary>
    ///   Is the item layouts list still loading?
    /// </summary>
    public bool ItemLayoutsListLoading { get; set; } = true;

    /// <summary>
    ///   The list of item layouts on the project, received from the API
    /// </summary>
    public List<ItemLayoutModel> ItemLayoutsList { get; set; } = [];

    /// <summary>
    ///   Fires when the component is loaded
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
        // TODO: Replace with different API call
        GetProjectSettingsResponse? settingsResponse = await apiClient.Project[ProjectId].Settings.GetAsync();
        if (settingsResponse == null)
        {
            logger.LogError("Failed to fetch project layouts for project {ProjectId}", ProjectId);
            return;
        }

        ItemLayoutsCount = settingsResponse.ItemLayoutCount;
        ItemLayoutsList = settingsResponse.ItemLayouts;
        ItemLayoutsListLoading = false;
    }

    /// <summary>
    ///   Gets the URL to create a new item layout
    /// </summary>
    /// <param name="projectId"></param>
    /// <returns></returns>
    protected static string GetCreateItemLayoutUrl(Guid projectId) => FrontEndUrls.Project.ItemLayout.ITEM_LAYOUT
        .Replace(FrontEndUrls.Project.PROJECT_ID_PLACEHOLDER, projectId.ToString());

    /// <summary>
    ///   Gets the URL to edit an item layout
    /// </summary>
    /// <param name="projectId"></param>
    /// <param name="layoutId"></param>
    /// <returns></returns>
    protected static string GetEditItemLayoutUrl(Guid projectId, Guid layoutId) => FrontEndUrls.Project.ItemLayout.ITEM_LAYOUT_WITH_ID
        .Replace(FrontEndUrls.Project.PROJECT_ID_PLACEHOLDER, projectId.ToString())
        .Replace(FrontEndUrls.Project.ItemLayout.ITEM_LAYOUT_ID_PLACEHOLDER, layoutId.ToString());
}
