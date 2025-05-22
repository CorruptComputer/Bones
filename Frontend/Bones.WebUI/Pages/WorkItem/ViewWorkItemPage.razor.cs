using System.Globalization;

namespace Bones.WebUI.Pages.WorkItem;

/// <summary>
///   Page for viewing a work item
/// </summary>
public partial class ViewWorkItemPage(BonesApiClient apiClient, ILogger<ViewWorkItemPage> logger) : ComponentBase
{
    /// <summary>
    ///   The ID of the work item to load in this dashboard
    /// </summary>
    [Parameter]
    [SupplyParameterFromQuery(Name = "work-item-id")]
    public required Guid WorkItemId { get; set; }

    /// <summary>
    ///   Did the request to the API result in an error?
    /// </summary>
    protected bool ApiError { get; set; } = false;

    private IOrderedEnumerable<WorkItemValueModel> _itemValues = Enumerable.Empty<WorkItemValueModel>().OrderBy(x => x.OrderNumber);

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
            GetWorkItemByIdResponse? workItemResponse = await apiClient.GetWorkItemByIdAsync(WorkItemId);

            if (workItemResponse is not null)
            {
                _itemValues = workItemResponse.ItemValues.OrderBy(x => x.OrderNumber);

                foreach (WorkItemValueModel itemValue in _itemValues)
                {
                    if (itemValue.ValueType == FieldType.DateTime)
                    {
                        logger.LogWarning(DateTimeOffset.Parse(itemValue.Value, CultureInfo.InvariantCulture).ToLocalTime().ToString(CultureInfo.CurrentCulture));
                    }
                }
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error fetching work item with ID {WorkItemId}", WorkItemId);
            ApiError = true;
        }

    }
}
