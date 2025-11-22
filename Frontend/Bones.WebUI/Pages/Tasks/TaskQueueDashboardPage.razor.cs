namespace Bones.WebUI.Pages.Tasks;

/// <summary>
///   Page for viewing a task queue
/// </summary>
/// <param name="apiClient"></param>
public partial class TaskQueueDashboardPage(BonesApiClient apiClient) : ComponentBase
{
    /// <summary>
    ///   The ID of the TaskQueue to load in this dashboard
    /// </summary>
    [Parameter]
    public Guid TaskQueueId { get; set; }

    /// <summary>
    ///   The name of the TaskQueue, received from the API
    /// </summary>
    public string? QueueName { get; set; }

    /// <summary>
    ///   The tasks in this queue, ordered by date added from oldest to newest
    /// </summary>
    public IOrderedEnumerable<DashboardTaskModel>? Tasks { get; set; }

    private bool TasksLoading { get; set; } = true;

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
        TasksLoading = true;
        GetTaskQueueDashboardResponse? dashboardResponse = await apiClient.TaskQueue[TaskQueueId].Dashboard.GetAsync();

        if (dashboardResponse is null)
        {
            return;
        }

        QueueName = dashboardResponse.QueueName;
        Tasks = dashboardResponse.Tasks.OrderBy(x => x.AddedToQueueDateTime);
        TasksLoading = false;
    }
}
