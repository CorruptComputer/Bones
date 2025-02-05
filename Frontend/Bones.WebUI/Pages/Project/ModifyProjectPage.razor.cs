using Bones.Api.Client;
using Bones.Shared.Consts;
using Microsoft.AspNetCore.Components;

namespace Bones.WebUI.Pages.Project;

/// <summary>
///   Modify a project page
/// </summary>
public partial class ModifyProjectPage : ComponentBase
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
    ///   The number of itemfields on the project, received from the API
    /// </summary>
    public int? ItemFieldsCount { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool ItemFieldsListLoading { get; set; } = true;

    /// <summary>
    /// 
    /// </summary>
    public List<ItemFieldModel> ItemFieldsList { get; set; } = [];

    /// <summary>
    ///   The number of itemlayouts on the project, received from the API
    /// </summary>
    public int? ItemLayoutsCount { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool ItemLayoutsListLoading { get; set; } = true;

    /// <summary>
    /// 
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
        ItemFieldsCount = settingsResponse.ItemFieldCount;
        ItemFieldsList = settingsResponse.ItemFields;
        ItemFieldsListLoading = false;
        ItemLayoutsCount = settingsResponse.ItemLayoutCount;
        ItemLayoutsList = settingsResponse.ItemLayouts;
        ItemLayoutsListLoading = false;
    }

    private static string GetCreateItemFieldUrl(Guid projectId) => FrontEndUrls.Project.ItemField.CREATE_FIELD
        .Replace("{ProjectId:guid}", projectId.ToString());

    private static string GetEditItemFieldUrl(Guid projectId, Guid fieldId) => FrontEndUrls.Project.ItemField.EDIT_FIELD
        .Replace("{ProjectId:guid}", projectId.ToString())
        .Replace("{ItemFieldId:guid}", fieldId.ToString());

    private static string GetCreateItemLayoutUrl(Guid projectId) => FrontEndUrls.Project.ItemLayout.CREATE_LAYOUT
        .Replace("{ProjectId:guid}", projectId.ToString());

    private static string GetEditItemLayoutUrl(Guid projectId, Guid layoutId) => FrontEndUrls.Project.ItemLayout.EDIT_LAYOUT
        .Replace("{ProjectId:guid}", projectId.ToString())
        .Replace("{ItemLayoutId:guid}", layoutId.ToString());
}