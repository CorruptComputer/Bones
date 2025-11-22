using System.ComponentModel.DataAnnotations;
using MudBlazor;
using ReQuesty.Runtime.Abstractions;
using FieldType = Bones.Api.Client.AutoGen.Models.FieldType;

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

    private Dictionary<int, SelectedItemFieldVersionModel> SelectedItemFields { get; set; } = [];

    private bool ItemAssigneesListLoading { get; set; } = true;
    private List<ItemAssigneeSlotModel> ItemAssigneesList { get; set; } = [];

    private string NewAssigneeName { get; set; } = string.Empty;
    private AssignmentType NewAssigneeAssignmentType { get; set; } = AssignmentType.User;
    private SelectionType NewAssigneeSelectionType { get; set; } = SelectionType.Single;

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
        GetProjectItemFieldsResponse? settingsResponse = await apiClient.Project[ProjectId].Fields.GetAsync();

        if (settingsResponse is null)
        {
            ApiError = true;
            return;
        }

        ItemFieldsList = settingsResponse.ItemFields;
        ItemFieldsListLoading = false;

        // Its a new layout, so no need to fetch existing data
        if (ItemLayoutId is null || ItemLayoutId == Guid.Empty)
        {
            SelectedItemFieldsLoading = false;
            ItemAssigneesListLoading = false;
            return;
        }

        GetItemLayoutVersionResponse? layoutResponse = await apiClient.ItemLayout.Layouts[ItemLayoutId.Value].Latest.GetAsync();

        if (layoutResponse is null)
        {
            ApiError = true;
            return;
        }

        LayoutName = layoutResponse.Name;
        LayoutUse = layoutResponse.LayoutUse!.Value; // Let it throw if its null, as this would mean the server sent an invalid value for the enum
        FriendlyIdPrefix = layoutResponse.FriendlyIdPrefix;
        SelectedItemFields = ItemFieldsList.Where(f => layoutResponse.FieldVersions.Any(fv => fv.Value == f.FieldVersionId))
            .ToDictionary(f => layoutResponse.FieldVersions.First(v => v.Value == f.FieldVersionId).Key, f => new SelectedItemFieldVersionModel(f.FieldVersionId, f.Name, f.IsRequired, f.Type!.Value));

        SelectedItemFieldsLoading = false;

        ItemAssigneesList = layoutResponse.AssigneeSlots;
        ItemAssigneesListLoading = false;
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
                await apiClient.ItemLayout.Layouts[ItemLayoutId.Value].PostAsync(new CreateItemLayoutVersionRequest()
                {
                    Name = LayoutName,
                    LayoutUse = LayoutUse,
                    FieldVersions = [..SelectedItemFields.Select(x => new Int32GuidKeyValuePair()
                    {
                        Key = x.Key,
                        Value = x.Value.FieldVersionId
                    })],
                    AssigneeSlots = ItemAssigneesList
                });
            }
            else
            {
                await apiClient.ItemLayout.Layouts.Create.PostAsync(new CreateItemLayoutRequest()
                {
                    ProjectId = ProjectId,
                    Name = LayoutName,
                    LayoutUse = LayoutUse,
                    FieldVersions = [..SelectedItemFields.Select(x => new Int32GuidKeyValuePair()
                    {
                        Key = x.Key,
                        Value = x.Value.FieldVersionId
                    })]
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

        SelectedItemFields.Add(SelectedItemFields.Count, new(field.FieldVersionId, field.Name, field.IsRequired, field.Type!.Value));
        SelectedItemFieldsCount = SelectedItemFields.Count;

        Form.Validate();
    }

    private void MoveFieldDown(Guid FieldVersionId)
    {
        if (!SelectedItemFields.Any(x => x.Value.FieldVersionId == FieldVersionId) || SelectedItemFields[(SelectedItemFields.Count - 1)].FieldVersionId == FieldVersionId)
        {
            return;
        }

        KeyValuePair<int, SelectedItemFieldVersionModel> fieldToMoveDown = SelectedItemFields.First(x => x.Value.FieldVersionId == FieldVersionId);
        KeyValuePair<int, SelectedItemFieldVersionModel> fieldToMoveUp = SelectedItemFields.First(x => x.Key == fieldToMoveDown.Key + 1);

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

        KeyValuePair<int, SelectedItemFieldVersionModel> fieldToMoveUp = SelectedItemFields.First(x => x.Value.FieldVersionId == FieldVersionId);
        KeyValuePair<int, SelectedItemFieldVersionModel> fieldToMoveDown = SelectedItemFields.First(x => x.Key == fieldToMoveUp.Key - 1);

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

        KeyValuePair<int, SelectedItemFieldVersionModel> fieldToRemove = SelectedItemFields.First(x => x.Value.FieldVersionId == FieldVersionId);

        for (int i = fieldToRemove.Key; i < SelectedItemFields.Count - 1; i++)
        {
            SelectedItemFields[i] = SelectedItemFields[i + 1];
        }

        SelectedItemFields.Remove(SelectedItemFields.Count - 1);
        SelectedItemFieldsCount = SelectedItemFields.Count;

        Form.Validate();
    }

    private void AddAssignee()
    {
        if (NewAssigneeName.Trim().Length == 0 || ItemAssigneesList.Any(x => x.Name == NewAssigneeName.Trim()))
        {
            return;
        }

        ItemAssigneesList.Add(new ItemAssigneeSlotModel()
        {
            Name = NewAssigneeName.Trim(),
            AssignmentType = NewAssigneeAssignmentType,
            SelectionType = NewAssigneeSelectionType,
            OrderNumber = ItemAssigneesList.Count
        });

        Form.Validate();
    }

    private void MoveAssigneeDown(string assigneeName)
    {
        if (!ItemAssigneesList.Any(x => x.Name == assigneeName))
        {
            return;
        }

        ItemAssigneeSlotModel assigneeToMoveDown = ItemAssigneesList.First(x => x.Name == assigneeName);
        ItemAssigneeSlotModel? assigneeToMoveUp = ItemAssigneesList.FirstOrDefault(x => x.OrderNumber == assigneeToMoveDown.OrderNumber + 1);

        if (assigneeToMoveUp == null)
        {
            return;
        }

        assigneeToMoveDown.OrderNumber++;
        assigneeToMoveUp.OrderNumber--;

        ItemAssigneesList = [.. ItemAssigneesList.OrderBy(a => a.OrderNumber)];

        Form.Validate();
    }

    private void MoveAssigneeUp(string assigneeName)
    {
        if (!ItemAssigneesList.Any(x => x.Name == assigneeName))
        {
            return;
        }

        ItemAssigneeSlotModel assigneeToMoveUp = ItemAssigneesList.First(x => x.Name == assigneeName);
        ItemAssigneeSlotModel? assigneeToMoveDown = ItemAssigneesList.FirstOrDefault(x => x.OrderNumber == assigneeToMoveUp.OrderNumber - 1);

        if (assigneeToMoveDown == null)
        {
            return;
        }

        assigneeToMoveUp.OrderNumber--;
        assigneeToMoveDown.OrderNumber++;

        ItemAssigneesList = [.. ItemAssigneesList.OrderBy(a => a.OrderNumber)];

        Form.Validate();
    }

    private void RemoveAssignee(string assigneeName)
    {
        if (!ItemAssigneesList.Any(x => x.Name == assigneeName))
        {
            return;
        }

        ItemAssigneeSlotModel assigneeToRemove = ItemAssigneesList.First(x => x.Name == assigneeName);

        ItemAssigneesList.Remove(assigneeToRemove);

        for (int i = assigneeToRemove.OrderNumber; i < ItemAssigneesList.Count; i++)
        {
            ItemAssigneesList[i].OrderNumber--;
        }

        Form.Validate();
    }

    /// <summary>
    ///   Model for the selected item field version
    /// </summary>
    /// <param name="FieldVersionId"></param>
    /// <param name="Name"></param>
    /// <param name="IsRequired"></param>
    /// <param name="Type"></param>
    public record SelectedItemFieldVersionModel(Guid FieldVersionId, string Name, bool IsRequired, FieldType Type);
}
