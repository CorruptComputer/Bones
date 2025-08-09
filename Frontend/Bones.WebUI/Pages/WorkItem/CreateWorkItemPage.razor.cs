using System.Text.Json;
using Bones.Shared.Consts;
using Bones.WebUI.Models;

namespace Bones.WebUI.Pages.WorkItem;

/// <summary>
///   Page for creating a new work item
/// </summary>
public partial class CreateWorkItemPage(BonesApiClient apiClient) : ComponentBase
{
    /// <summary>
    ///   The ID of the project to load in this dashboard
    /// </summary>
    [Parameter]
    [SupplyParameterFromQuery(Name = "work-item-queue-id")]
    public Guid? WorkItemQueueId { get; set; }

    /// <summary>
    ///   Is the work item queue ID provided in the query string?
    /// </summary>
    protected bool WorkItemQueueIdProvidedInQueryString { get; set; }

    /// <summary>
    ///   The projects dropdown values
    /// </summary>
    protected List<DropDownModel> Projects { get; set; } = [
        new()
        {
            DisplayStr = "(loading)",
            Id = null
        }];

    /// <summary>
    ///   The project selected by the user
    /// </summary>
    protected Guid? SelectedProject { get; set; }

    /// <summary>
    ///   The initiatives dropdown values
    /// </summary>
    protected List<DropDownModel> Initiatives { get; set; } = [
        new()
        {
            DisplayStr = "(loading)",
            Id = null
        }];

    /// <summary>
    ///   The initiative selected by the user
    /// </summary>
    protected Guid? SelectedInitiative { get; set; }

    /// <summary>
    ///   The work item queues dropdown values
    /// </summary>
    protected List<DropDownModel> WorkItemQueues { get; set; } = [
        new()
        {
            DisplayStr = "(loading)",
            Id = null
        }];

    /// <summary>
    ///   The work item queue selected by the user
    /// </summary>
    protected Guid? SelectedWorkItemQueue { get; set; }

    /// <summary>
    ///   The work item layout dropdown values
    /// </summary>
    protected List<DropDownModel> WorkItemLayouts { get; set; } = [
        new()
        {
            DisplayStr = "(loading)",
            Id = null
        }];

    /// <summary>
    ///   The work item layout selected by the user
    /// </summary>
    protected Guid? SelectedWorkItemLayout { get; set; }


    /// <summary>
    ///   Did the request to the API result in an error?
    /// </summary>
    protected bool ApiError { get; set; } = false;

    /// <summary>
    ///   Is the form valid?
    /// </summary>
    protected bool FormValid { get; set; }

    /// <summary>
    ///   The issues with the users inputs
    /// </summary>
    protected string[] ValidationErrors { get; set; } = [];

    /// <summary>
    ///   The name of the work item queue, received from the API
    /// </summary>
    protected string? WorkItemQueueName { get; set; }

    private string _workItemTitle { get; set; } = string.Empty;

    private Dictionary<Guid, bool?> _boolValues { get; set; } = [];

    private Dictionary<Guid, DateTime?> _dateTimeValues { get; set; } = [];

    private Dictionary<Guid, TimeSpan?> _timeSpanValues { get; set; } = [];

    private Dictionary<Guid, double?> _decimalValues { get; set; } = [];

    private Dictionary<Guid, long?> _integerValues { get; set; } = [];

    private Dictionary<Guid, string> _stringValues { get; set; } = [];

    /// <summary>
    ///   Fires when the page is loaded
    /// </summary>
    /// <returns></returns>
    protected override async Task OnInitializedAsync()
    {
        WorkItemQueueIdProvidedInQueryString = WorkItemQueueId.HasValue;
        await FetchFromAPI();

        await base.OnInitializedAsync();
    }

    /// <summary>
    ///   Fires if the same page but with a different parameter is loaded
    /// </summary>
    /// <returns></returns>
    protected override async Task OnParametersSetAsync()
    {
        WorkItemQueueIdProvidedInQueryString = WorkItemQueueId.HasValue;
        await FetchFromAPI();

        await base.OnParametersSetAsync();
    }

    private async Task FetchFromAPI()
    {
        if (WorkItemQueueIdProvidedInQueryString && WorkItemQueueId.HasValue)
        {
            GetWorkItemQueueByIdResponse itemQueue = await apiClient.GetWorkItemQueueByIdAsync(WorkItemQueueId.Value);
            SelectedProject = itemQueue.ProjectId;
            SelectedInitiative = itemQueue.InitiativeId;
            WorkItemQueueName = itemQueue.QueueName;
            SelectedWorkItemQueue = WorkItemQueueId.Value;
            await GetWorkItemLayouts();
        }
        else
        {
            await GetProjects();
        }
    }

    private async Task GetProjects()
    {
        List<GetProjectQuickSelectResponse> resp = await apiClient.GetProjectQuickSelectAsync();

        Projects = [.. resp.Select(x => new DropDownModel
        {
            DisplayStr = x.ProjectName,
            Id = x.ProjectId
        })];
    }

    private async Task GetInitiatives(Guid projectId)
    {
        List<GetInitiativesInProjectResponse> resp = await apiClient.GetInitiativesInProjectAsync(projectId);

        Initiatives = [.. resp.Select(x => new DropDownModel
        {
            DisplayStr = x.Name,
            Id = x.Id
        })];
    }

    private async Task GetWorkItemQueues(Guid initiativeId)
    {
        List<GetWorkItemQueuesInInitiativeResponse> resp = await apiClient.GetWorkItemQueuesInInitiativeAsync(initiativeId);

        WorkItemQueues = [.. resp.Select(x => new DropDownModel
        {
            DisplayStr = x.QueueName,
            Id = x.QueueId
        })];
    }

    private async Task GetWorkItemLayouts()
    {
        if (!SelectedProject.HasValue)
        {
            return;
        }

        List<GetProjectLayoutsResponse> resp = await apiClient.GetProjectLayoutsAsync(SelectedProject.Value, ItemLayoutUse.WorkItems);

        WorkItemLayouts = [.. resp.Select(x => new DropDownModel
        {
            DisplayStr = $"{x.FriendlyIdPrefix} - {x.LayoutName}",
            Id = x.LayoutId
        })];
    }

    /// <summary>
    ///   Event for when the selected project is changed
    /// </summary>
    /// <param name="selectedProject"></param>
    /// <returns></returns>
    protected async Task OnSelectedProjectChanged(Guid? selectedProject)
    {
        if (selectedProject.HasValue)
        {
            SelectedProject = selectedProject.Value;
            await GetInitiatives(SelectedProject.Value);
        }
    }

    /// <summary>
    ///   Event for when the selected initiative is changed
    /// </summary>
    /// <param name="selectedInitiative"></param>
    /// <returns></returns>
    protected async Task OnSelectedInitiativeChanged(Guid? selectedInitiative)
    {
        if (selectedInitiative.HasValue)
        {
            SelectedInitiative = selectedInitiative.Value;
            await GetWorkItemQueues(SelectedInitiative.Value);
        }
    }

    /// <summary>
    ///   Event for when the selected work item queue is changed
    /// </summary>
    /// <param name="selectedQueue"></param>
    /// <returns></returns>
    protected async Task OnSelectedWorkItemQueueChanged(Guid? selectedQueue)
    {
        if (selectedQueue.HasValue)
        {
            SelectedWorkItemQueue = selectedQueue.Value;
            await GetWorkItemLayouts();
        }
    }

    /// <summary>
    ///   Event for when the selected work item layout is changed
    /// </summary>
    /// <param name="selectedLayout"></param>
    /// <returns></returns>
    protected async Task OnSelectedLayoutChanged(Guid? selectedLayout)
    {
        if (selectedLayout.HasValue)
        {
            SelectedWorkItemLayout = selectedLayout.Value;
            await Task.CompletedTask; //GetCurrentLayoutVersionFields();
        }
    }


}
