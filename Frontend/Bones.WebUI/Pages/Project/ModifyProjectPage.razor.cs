using Bones.Shared.Consts;
using Bones.Shared.Enums;

namespace Bones.WebUI.Pages.Project;

/// <summary>
///   Modify a project page
/// </summary>
public partial class ModifyProjectPage(BonesApiClient ApiClient) : ComponentBase
{
    /// <summary>
    ///   The ID of the project to load
    /// </summary>
    [Parameter]
    public Guid ProjectId { get; set; }

    /// <summary>
    ///   The name of the project, received from the API
    /// </summary>
    public string? ProjectName { get; set; }

    /// <summary>
    ///   The type of owner for the project, received from the API
    /// </summary>
    public OwnershipType? OwnerType { get; set; }

    /// <summary>
    ///   The ID of the owner of the project, received from the API
    /// </summary>
    public Guid? OwnerId { get; set; }

    /// <summary>
    ///   The name of the owner of the project, received from the API
    /// </summary>
    public string? OwnerName { get; set; }

    /// <summary>
    ///   The number of itemfields on the project, received from the API
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
    ///   The number of itemlayouts on the project, received from the API
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
        GetProjectSettingsResponse settingsResponse = await ApiClient.GetProjectSettingsAsync(ProjectId);
        ProjectName = settingsResponse.ProjectName;
        OwnerType = settingsResponse.OwnerType;
        OwnerId = settingsResponse.OwnerId;
        OwnerName = settingsResponse.OwnerDisplayName;
        ItemFieldsCount = settingsResponse.ItemFieldCount;
        ItemFieldsList = settingsResponse.ItemFields;
        ItemFieldsListLoading = false;
        ItemLayoutsCount = settingsResponse.ItemLayoutCount;
        ItemLayoutsList = settingsResponse.ItemLayouts;
        ItemLayoutsListLoading = false;
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