using Bones.Api.Client;
using Microsoft.AspNetCore.Components;

namespace Bones.WebUI.Pages.Project;

/// <summary>
///   Dashboard for projects
/// </summary>
public partial class InitiativeDashboardPage(BonesApiClient ApiClient) : ComponentBase
{
    /// <summary>
    ///   The ID of the project to load in this dashboard
    /// </summary>
    [Parameter]
    public Guid ProjectId { get; set; }

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
    ///   The number of queues on the initiative, received from the API
    /// </summary>
    public int? QueueCount { get; set; }

    /// <summary>
    ///   Is the queue list still loading from the API?
    /// </summary>
    public bool QueueListLoading { get; set; } = true;

    /// <summary>
    ///   The list of queues on the initiative, received from the API
    /// </summary>
    public List<WorkItemQueueListModel> QueueList { get; set; } = [];

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
        GetInitiativeDashboardResponse dashboardResponse = await ApiClient.GetInitiativeDashboardAsync(ProjectId, InitiativeId);
        InitiativeName = dashboardResponse.InitiativeName;
        QueueCount = dashboardResponse.WorkItemQueueCount;
        QueueList = dashboardResponse.WorkItemQueues;
        QueueListLoading = false;
    }
}