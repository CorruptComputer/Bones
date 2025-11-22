namespace Bones.WebUI.Pages.Project;

/// <summary>
///   Dashboard for projects
/// </summary>
public partial class InitiativeDashboardPage(BonesApiClient ApiClient) : ComponentBase
{
    /// <summary>
    ///   The ID of the initiative to load in this dashboard
    /// </summary>
    [Parameter]
    public Guid InitiativeId { get; set; }

    /// <summary>
    ///   The name of the initiative, received from the API
    /// </summary>
    public string? InitiativeName { get; set; }

    /// <summary>
    ///   The number of task queues on the initiative, received from the API
    /// </summary>
    public int? TaskQueueCount { get; set; }

    /// <summary>
    ///   Is the task queue list still loading from the API?
    /// </summary>
    public bool TaskQueueListLoading { get; set; } = true;

    /// <summary>
    ///   The list of task queues on the initiative, received from the API
    /// </summary>
    public List<TaskQueueListModel> TaskQueueList { get; set; } = [];

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
        GetInitiativeDashboardResponse? dashboardResponse = await ApiClient.Initiative[InitiativeId].Dashboard.GetAsync();

        if (dashboardResponse is null)
        {
            return;
        }

        InitiativeName = dashboardResponse.InitiativeName;
        TaskQueueCount = dashboardResponse.TaskQueueCount;
        TaskQueueList = dashboardResponse.TaskQueues;
        TaskQueueListLoading = false;
    }
}