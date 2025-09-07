namespace Bones.WebUI.Pages.WorkItem;

/// <summary>
///   Page for viewing a work item queue
/// </summary>
/// <param name="apiClient"></param>
public partial class WorkItemQueueDashboardPage(BonesApiClient apiClient) : ComponentBase
{
    /// <summary>
    ///   The ID of the WorkItemQueue to load in this dashboard
    /// </summary>
    [Parameter]
    public Guid WorkItemQueueId { get; set; }

    /// <summary>
    ///   The name of the WorkItemQueue, received from the API
    /// </summary>
    public string? QueueName { get; set; }

    /// <summary>
    ///   The work items in this queue, ordered by date added from oldest to newest
    /// </summary>
    public IOrderedEnumerable<DashboardWorkItemModel>? WorkItems { get; set; }

    private bool WorkItemsLoading { get; set; } = true;

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
        WorkItemsLoading = true;
        GetWorkItemQueueDashboardResponse? dashboardResponse = await apiClient.WorkItemQueue[WorkItemQueueId].Dashboard.GetAsync();

        if (dashboardResponse is null)
        {
            return;
        }

        QueueName = dashboardResponse.QueueName;
        WorkItems = dashboardResponse.WorkItems.OrderBy(x => x.AddedToQueueDateTime);
        WorkItemsLoading = false;
    }
}
