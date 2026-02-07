namespace Bones.WebUI.Components.Project.Modify;

/// <summary>
///   Component for viewing a project's item fields
/// </summary>
public partial class ProjectFieldsSummary(BonesApiClient apiClient, ILogger<ProjectFieldsSummary> logger) : ComponentBase
{
    /// <summary>
    ///   The ID of the project to load
    /// </summary>
    [Parameter, EditorRequired]
    public required Guid ProjectId { get; set; }

    /// <summary>
    ///   The number of item fields on the project, received from the API
    /// </summary>
    public int? ItemFieldsCount { get; set; }

    /// <summary>
    ///   Is the item fields list still loading?
    /// </summary>
    public bool ItemFieldsListLoading { get; set; } = true;

    /// <summary>
    ///   The list of item fields on the project, received from the API
    /// </summary>
    public List<ItemFieldModel> ItemFieldsList { get; set; } = [];

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
            logger.LogError("Failed to fetch project fields for project {ProjectId}", ProjectId);
            return;
        }

        ItemFieldsCount = settingsResponse.ItemFieldCount;
        ItemFieldsList = settingsResponse.ItemFields;
        ItemFieldsListLoading = false;
    }

    /// <summary>
    ///   Gets the URL to create a new item field
    /// </summary>
    /// <param name="projectId"></param>
    /// <returns></returns>
    protected static string GetCreateItemFieldUrl(Guid projectId) => FrontEndUrls.Project.ItemField.ITEM_FIELD
        .Replace(FrontEndUrls.Project.PROJECT_ID_PLACEHOLDER, projectId.ToString());

    /// <summary>
    ///   Gets the URL to edit an item field
    /// </summary>
    /// <param name="projectId"></param>
    /// <param name="fieldId"></param>
    /// <returns></returns>
    protected static string GetEditItemFieldUrl(Guid projectId, Guid fieldId) => FrontEndUrls.Project.ItemField.ITEM_FIELD_WITH_ID
        .Replace(FrontEndUrls.Project.PROJECT_ID_PLACEHOLDER, projectId.ToString())
        .Replace(FrontEndUrls.Project.ItemField.ITEM_FIELD_ID_PLACEHOLDER, fieldId.ToString());
}
