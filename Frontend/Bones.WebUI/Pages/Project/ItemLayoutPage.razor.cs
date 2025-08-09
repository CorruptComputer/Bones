using System.ComponentModel.DataAnnotations;
using Bones.Shared.Consts;
using Bones.Shared.Enums;
using MudBlazor;

namespace Bones.WebUI.Pages.Project;

/// <summary>
///   Page to create or edit an item layout
/// </summary>
/// <param name="apiClient"></param>
/// <param name="navManager"></param>
/// <param name="logger"></param>
public partial class ItemLayoutPage(BonesApiClient apiClient, NavigationManager navManager, ILogger<ItemLayoutPage> logger) : ComponentBase
{
    /// <summary>
    ///   The ID of the project to load in this dashboard
    /// </summary>
    [Parameter]
    public Guid ProjectId { get; set; }

    /// <summary>
    ///   The ID of the item layout to load on this page
    /// </summary>
    [Parameter]
    [SupplyParameterFromQuery(Name = "itemLayoutId")]
    public Guid? ItemLayoutId { get; set; }

    private string TitleText => ItemLayoutId.HasValue ? "Edit Item Layout" : "Create Item Layout";

    private string SaveButtonText => ItemLayoutId.HasValue ? "Save" : "Create";

    private bool ApiError { get; set; } = false;

    private bool ItemFieldsListLoading { get; set; } = true;

    private MudForm Form { get; set; } = new();

    private bool FormValid { get; set; }

    private string[] ValidationErrors { get; set; } = [];

    private List<ProjectItemFieldModel> ItemFieldsList { get; set; } = [];

    private string LayoutName { get; set; } = string.Empty;

    private ItemLayoutUse LayoutUse { get; set; } = ItemLayoutUse.None;

    private string FriendlyIdPrefix { get; set; } = string.Empty;

    private bool SelectedItemFieldsLoading { get; set; } = true;

    private Dictionary<uint, SelectedItemFieldVersionModel> SelectedItemFields { get; set; } = [];

    // This is a hack so we can bind this to the UI to show a validation error for
    [Range(1, int.MaxValue, ErrorMessage = "At least one field must be selected")]
    private int SelectedItemFieldsCount { get; set; } = 0;

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
        GetProjectItemFieldsResponse settingsResponse = await apiClient.GetProjectItemFieldsAsync(ProjectId);
        ItemFieldsList = settingsResponse.ItemFields;
        ItemFieldsListLoading = false;

        if (ItemLayoutId == null || ItemLayoutId == Guid.Empty)
        {
            SelectedItemFieldsLoading = false;
            return;
        }

        GetItemLayoutVersionResponse layoutResponse = await apiClient.GetLatestItemLayoutVersionAsync(ItemLayoutId.Value);
        LayoutName = layoutResponse.Name;
        LayoutUse = layoutResponse.LayoutUse;
        FriendlyIdPrefix = layoutResponse.FriendlyIdPrefix;
        SelectedItemFields = ItemFieldsList.Where(f => layoutResponse.FieldVersions.ContainsValue(f.FieldVersionId))
            .ToDictionary(f => uint.Parse(layoutResponse.FieldVersions.First(v => v.Value == f.FieldVersionId).Key), f => new SelectedItemFieldVersionModel(f.FieldVersionId, f.Name, f.IsRequired, f.Type));

        SelectedItemFieldsLoading = false;
    }

    private async Task SendCreateRequestAsync()
    {

        if (SelectedItemFields.Count == 0)
        {
            return;
        }

        try
        {
            ApiError = false;

            if (ItemLayoutId.HasValue)
            {
                await apiClient.CreateItemLayoutVersionAsync(ProjectId, ItemLayoutId.Value, new CreateItemLayoutVersionRequest()
                {
                    Name = LayoutName,
                    LayoutUse = LayoutUse,
                    FieldVersions = SelectedItemFields.ToDictionary(x => x.Key.ToString(), x => x.Value.FieldVersionId)
                });
            }
            else
            {
                await apiClient.CreateItemLayoutAsync(ProjectId, new CreateItemLayoutRequest()
                {
                    Name = LayoutName,
                    LayoutUse = LayoutUse,
                    FriendlyIdPrefix = FriendlyIdPrefix,
                    FieldVersions = SelectedItemFields.ToDictionary(x => x.Key.ToString(), x => x.Value.FieldVersionId)
                });
            }

            navManager.NavigateTo(FrontEndUrls.Project.MODIFY_PROJECT.Replace(FrontEndUrls.Project.PROJECT_ID_PLACEHOLDER, ProjectId.ToString()));
        }
        catch (ApiException ex)
        {
            logger.LogError(ex, "Error");
            ApiError = true;
        }
    }

    private void AddField(Guid FieldVersionId)
    {
        if (SelectedItemFields.Any(x => x.Value.FieldVersionId == FieldVersionId))
        {
            return;
        }

        ProjectItemFieldModel? field = ItemFieldsList.FirstOrDefault(x => x.FieldVersionId == FieldVersionId);

        if (field == null)
        {
            return;
        }

        SelectedItemFields.Add((uint)SelectedItemFields.Count, new(field.FieldVersionId, field.Name, field.IsRequired, field.Type));
        SelectedItemFieldsCount = SelectedItemFields.Count;

        Form.Validate();
    }

    private void MoveFieldDown(Guid FieldVersionId)
    {
        if (!SelectedItemFields.Any(x => x.Value.FieldVersionId == FieldVersionId) || SelectedItemFields[(uint)(SelectedItemFields.Count - 1)].FieldVersionId == FieldVersionId)
        {
            return;
        }

        KeyValuePair<uint, SelectedItemFieldVersionModel> fieldToMoveDown = SelectedItemFields.First(x => x.Value.FieldVersionId == FieldVersionId);
        KeyValuePair<uint, SelectedItemFieldVersionModel> fieldToMoveUp = SelectedItemFields.First(x => x.Key == fieldToMoveDown.Key + 1);

        SelectedItemFields[fieldToMoveDown.Key] = fieldToMoveUp.Value;
        SelectedItemFields[fieldToMoveUp.Key] = fieldToMoveDown.Value;
        SelectedItemFieldsCount = SelectedItemFields.Count;

        Form.Validate();
    }

    private void MoveFieldUp(Guid FieldVersionId)
    {
        if (!SelectedItemFields.Any(x => x.Value.FieldVersionId == FieldVersionId) || SelectedItemFields[0].FieldVersionId == FieldVersionId)
        {
            return;
        }

        KeyValuePair<uint, SelectedItemFieldVersionModel> fieldToMoveUp = SelectedItemFields.First(x => x.Value.FieldVersionId == FieldVersionId);
        KeyValuePair<uint, SelectedItemFieldVersionModel> fieldToMoveDown = SelectedItemFields.First(x => x.Key == fieldToMoveUp.Key - 1);

        SelectedItemFields[fieldToMoveUp.Key] = fieldToMoveDown.Value;
        SelectedItemFields[fieldToMoveDown.Key] = fieldToMoveUp.Value;
        SelectedItemFieldsCount = SelectedItemFields.Count;

        Form.Validate();
    }

    private void RemoveField(Guid FieldVersionId)
    {
        if (!SelectedItemFields.Any(x => x.Value.FieldVersionId == FieldVersionId))
        {
            return;
        }

        if (SelectedItemFields.First(x => x.Value.FieldVersionId == FieldVersionId).Key == SelectedItemFields.Count - 1)
        {
            SelectedItemFields.Remove(SelectedItemFields.First(x => x.Value.FieldVersionId == FieldVersionId).Key);
            return;
        }

        KeyValuePair<uint, SelectedItemFieldVersionModel> fieldToRemove = SelectedItemFields.First(x => x.Value.FieldVersionId == FieldVersionId);

        for (uint i = fieldToRemove.Key; i < SelectedItemFields.Count - 1; i++)
        {
            SelectedItemFields[i] = SelectedItemFields[i + 1];
        }

        SelectedItemFields.Remove((uint)(SelectedItemFields.Count - 1));
        SelectedItemFieldsCount = SelectedItemFields.Count;

        Form.Validate();
    }

    /// <summary>
    ///   Model for the selected item field version
    /// </summary>
    /// <param name="FieldVersionId"></param>
    /// <param name="Name"></param>
    /// <param name="IsRequired"></param>
    /// <param name="Type"></param>
    public record SelectedItemFieldVersionModel(Guid FieldVersionId, string Name, bool IsRequired, Shared.Enums.FieldType Type);
}
