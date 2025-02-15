using Bones.Api.Client;
using Bones.Shared.Consts;
using Microsoft.AspNetCore.Components;

namespace Bones.WebUI.Pages.Project;

/// <summary>
///   Create Item Layout page
/// </summary>
public partial class CreateItemLayoutPage : ComponentBase
{
    /// <summary>
    ///   The ID of the project to load in this dashboard
    /// </summary>
    [Parameter]
    public Guid ProjectId { get; set; }

    /// <summary>
    ///   Did the request to the API result in an error?
    /// </summary>
    public bool ApiError { get; set; } = false;

    /// <summary>
    ///   Is the form valid?
    /// </summary>
    public bool FormValid { get; set; }

    /// <summary>
    ///   The issues with the users inputs
    /// </summary>
    public string[] ValidationErrors { get; set; } = [];

    /// <summary>
    /// 
    /// </summary>
    public string LayoutName { get; set; } = string.Empty;

    /// <summary>
    /// 
    /// </summary>
    public bool EnabledForWorkItems { get; set; } = true;

    /// <summary>
    /// 
    /// </summary>
    public bool EnabledForAssets { get; set; } = true;

    /// <summary>
    /// 
    /// </summary>
    public string FriendlyIdPrefix { get; set; } = string.Empty;

    /// <summary>
    /// 
    /// </summary>
    public bool ItemFieldsListLoading { get; set; } = true;

    /// <summary>
    /// 
    /// </summary>
    public List<ProjectItemFieldModel> ItemFieldsList { get; set; } = [];

    /// <summary>
    /// 
    /// </summary>
    public List<Guid> SelectedItemFields { get; set; } = [];

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
        GetProjectItemFieldsResponse settingsResponse = await ApiClient.GetProjectItemFieldsAsync(ProjectId);
        ItemFieldsList = settingsResponse.ItemFields;
        ItemFieldsListLoading = false;
    }

    /// <summary>
    ///   Send the request to create to the API, if it errors tell the user what went wrong.
    /// </summary>
    public async Task SendCreateRequestAsync()
    {
        if (!FormValid)
        {
            return;
        }

        try
        {
            ApiError = false;

            ItemLayoutUses enabledFor = ItemLayoutUses.None;
            if (EnabledForWorkItems)
            {
                enabledFor |= ItemLayoutUses.WorkItems;
            }

            if (EnabledForAssets)
            {
                enabledFor |= ItemLayoutUses.Assets;
            }

            await ApiClient.CreateItemLayoutAsync(ProjectId, new CreateItemLayoutRequest()
            {
                Name = LayoutName,
                EnabledFor = enabledFor,
                FriendlyIdPrefix = FriendlyIdPrefix,
                FieldVersions = SelectedItemFields
            });

            NavManager.NavigateTo(FrontEndUrls.Project.MODIFY_PROJECT.Replace("{ProjectId:guid}", ProjectId.ToString()));
        }
        catch (ApiException ex)
        {
            Logger.LogError(ex, "Error while creating initiative");
            ApiError = true;
        }
    }

    /// <summary>
    ///   Adds an item field to the layout
    /// </summary>
    /// <param name="FieldVersionId"></param>
    protected void AddField_OnClick(Guid FieldVersionId)
    {
        if (SelectedItemFields.Contains(FieldVersionId))
        {
            return;
        }

        SelectedItemFields.Add(FieldVersionId);
    }

    /// <summary>
    ///   Removes an item field to the layout
    /// </summary>
    /// <param name="FieldVersionId"></param>
    protected void RemoveField_OnClick(Guid FieldVersionId)
    {
        if (!SelectedItemFields.Contains(FieldVersionId))
        {
            return;
        }

        SelectedItemFields.Remove(FieldVersionId);
    }
}