using System.Globalization;
using System.Net;

namespace Bones.WebUI.Pages.WorkItem;

/// <summary>
///   Page for viewing a work item
/// </summary>
public partial class ViewWorkItemPage(BonesApiClient apiClient, NavigationManager navManager, ILogger<ViewWorkItemPage> logger) : ComponentBase
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

    /// <summary>
    ///   Item values displayed on the page for this work item, if editing is enabled: the values that are being changed.
    /// </summary>
    private IOrderedEnumerable<WorkItemValueModel> _itemValues = Enumerable.Empty<WorkItemValueModel>().OrderBy(x => x.OrderNumber);

    /// <summary>
    ///   Do not update this after its been loaded from the API.
    /// </summary>
    private IOrderedEnumerable<WorkItemValueModel> _originalValues = Enumerable.Empty<WorkItemValueModel>().OrderBy(x => x.OrderNumber);

    private Guid _queueId = Guid.Empty;
    private string _queueName = string.Empty;
    private string _addedToQueueDateTime = string.Empty;

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
                _originalValues = _itemValues = workItemResponse.ItemValues.OrderBy(x => x.OrderNumber);
                _queueId = workItemResponse.WorkItemQueueId;
                _queueName = workItemResponse.WorkItemQueueName;
                _addedToQueueDateTime = workItemResponse.AddedToQueueDateTime.ToLocalTime().ToString(CultureInfo.CurrentCulture);
            }
        }
        catch (ApiException<ErrorResponse> ex) when (ex.StatusCode == (int)HttpStatusCode.NotFound)
        {
            logger.LogWarning("Work item with ID {WorkItemId} not found, redirecting to home", WorkItemId);
            navManager.NavigateTo("/");
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error fetching work item with ID {WorkItemId}", WorkItemId);
            ApiError = true;
        }
    }
}
