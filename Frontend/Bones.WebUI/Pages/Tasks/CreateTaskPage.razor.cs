using Bones.WebUI.Models;

namespace Bones.WebUI.Pages.Tasks;

/// <summary>
///   Page for creating a new task
/// </summary>
public partial class CreateTaskPage(BonesApiClient apiClient) : ComponentBase
{
    /// <summary>
    ///   The ID of the project to load in this dashboard
    /// </summary>
    [Parameter]
    [SupplyParameterFromQuery(Name = "task-queue-id")]
    public Guid? TaskQueueId { get; set; }

    /// <summary>
    ///   Is the task queue ID provided in the query string?
    /// </summary>
    protected bool TaskQueueIdProvidedInQueryString { get; set; }

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
    ///   The task queues dropdown values
    /// </summary>
    protected List<DropDownModel> TaskQueues { get; set; } = [
        new()
        {
            DisplayStr = "(loading)",
            Id = null
        }];

    /// <summary>
    ///   The task queue selected by the user
    /// </summary>
    protected Guid? SelectedTaskQueue { get; set; }

    /// <summary>
    ///   The task layout dropdown values
    /// </summary>
    protected List<DropDownModel> TaskLayouts { get; set; } = [
        new()
        {
            DisplayStr = "(loading)",
            Id = null
        }];

    /// <summary>
    ///   The task layout selected by the user
    /// </summary>
    protected Guid? SelectedTaskLayout { get; set; }


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
    ///   The name of the task queue, received from the API
    /// </summary>
    protected string? TaskQueueName { get; set; }

    private string _taskTitle { get; set; } = string.Empty;

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
        TaskQueueIdProvidedInQueryString = TaskQueueId.HasValue;
        await FetchFromAPI();

        await base.OnInitializedAsync();
    }

    /// <summary>
    ///   Fires if the same page but with a different parameter is loaded
    /// </summary>
    /// <returns></returns>
    protected override async Task OnParametersSetAsync()
    {
        TaskQueueIdProvidedInQueryString = TaskQueueId.HasValue;
        await FetchFromAPI();

        await base.OnParametersSetAsync();
    }

    private async Task FetchFromAPI()
    {
        if (TaskQueueIdProvidedInQueryString && TaskQueueId.HasValue)
        {
            GetTaskQueueByIdResponse? itemQueue = await apiClient.TaskQueue[TaskQueueId.Value].GetAsync();
            if (itemQueue is null)
            {
                return;
            }
            SelectedProject = itemQueue.ProjectId;
            SelectedInitiative = itemQueue.InitiativeId;
            TaskQueueName = itemQueue.QueueName;
            SelectedTaskQueue = TaskQueueId.Value;
            await GetTaskLayouts();
        }
        else
        {
            await GetProjects();
        }
    }

    private async Task GetProjects()
    {
        List<GetProjectQuickSelectResponse>? resp = await apiClient.Project.Projects.QuickSelect.GetAsync();
        if (resp is null)
        {
            return;
        }

        Projects = [.. resp.Select(x => new DropDownModel
        {
            DisplayStr = x.ProjectName,
            Id = x.ProjectId
        })];
    }

    private async Task GetInitiatives(Guid projectId)
    {
        List<GetInitiativesInProjectResponse>? resp = await apiClient.Project[projectId].Initiatives.GetAsync();
        if (resp is null)
        {
            return;
        }

        Initiatives = [.. resp.Select(x => new DropDownModel
        {
            DisplayStr = x.Name,
            Id = x.Id
        })];
    }

    private async Task GetTaskQueues(Guid initiativeId)
    {
        List<GetTaskQueuesInInitiativeResponse>? resp = await apiClient.Initiative[initiativeId].TaskQueues.GetAsync();
        if (resp is null)
        {
            return;
        }

        TaskQueues = [.. resp.Select(x => new DropDownModel
        {
            DisplayStr = x.TaskQueueName,
            Id = x.TaskQueueId
        })];
    }

    private async Task GetTaskLayouts()
    {
        if (!SelectedProject.HasValue)
        {
            return;
        }

        List<GetProjectLayoutsResponse>? resp = await apiClient.Project[SelectedProject.Value].Layouts.GetAsync(req => req.QueryParameters.LayoutUse = ItemLayoutUse.Tasks);
        if (resp is null)
        {
            return;
        }

        TaskLayouts = [.. resp.Select(x => new DropDownModel
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
            await GetTaskQueues(SelectedInitiative.Value);
        }
    }

    /// <summary>
    ///   Event for when the selected task queue is changed
    /// </summary>
    /// <param name="selectedQueue"></param>
    /// <returns></returns>
    protected async Task OnSelectedTaskQueueChanged(Guid? selectedQueue)
    {
        if (selectedQueue.HasValue)
        {
            SelectedTaskQueue = selectedQueue.Value;
            await GetTaskLayouts();
        }
    }

    /// <summary>
    ///   Event for when the selected task layout is changed
    /// </summary>
    /// <param name="selectedLayout"></param>
    /// <returns></returns>
    protected async Task OnSelectedLayoutChanged(Guid? selectedLayout)
    {
        if (selectedLayout.HasValue)
        {
            SelectedTaskLayout = selectedLayout.Value;
            await Task.CompletedTask; //GetCurrentLayoutVersionFields();
        }
    }


}
