using System.Globalization;
using System.Net;
using Bones.WebUI.Models;
using ReQuesty.Runtime.Abstractions;

namespace Bones.WebUI.Pages.Tasks;

/// <summary>
///   Page for viewing a task
/// </summary>
public partial class ViewTaskPage(BonesApiClient apiClient, NavigationManager navManager, ILogger<ViewTaskPage> logger) : ComponentBase
{
    /// <summary>
    ///   The ID of the task to load in this dashboard
    /// </summary>
    [Parameter]
    [SupplyParameterFromQuery(Name = "task-id")]
    public required Guid TaskId { get; set; }

    /// <summary>
    ///   Did the request to the API result in an error?
    /// </summary>
    protected bool ApiError { get; set; } = false;

    private int _currentVersion = 0;
    private Guid _projectId = Guid.Empty;
    private Guid _queueId = Guid.Empty;
    private string _queueName = string.Empty;
    private string _addedToQueueDateTime = string.Empty;
    private bool _moveToQueue = false;
    private string _taskTitle { get; set; } = string.Empty;

    private List<DropDownModel> _initiatives { get; set; } = [
        new()
        {
            DisplayStr = "(loading)",
            Id = null
        }];

    private Guid? _selectedInitiative { get; set; }

    private List<DropDownModel> _taskQueues { get; set; } = [
        new()
        {
            DisplayStr = "(loading)",
            Id = null
        }];

    private Guid? _selectedNewTaskQueue { get; set; }

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
            GetTaskByIdResponse? taskResponse = await apiClient.Task[TaskId].GetAsync();

            if (taskResponse is not null)
            {
                _projectId = taskResponse.ProjectId;
                _queueId = taskResponse.TaskQueueId;
                _queueName = taskResponse.TaskQueueName;
                _addedToQueueDateTime = taskResponse.AddedToQueueDateTime.ToLocalTime().ToString(CultureInfo.CurrentCulture);
                _currentVersion = taskResponse.CurrentVersion;
                _taskTitle = taskResponse.Title;

                await GetInitiatives();
            }
        }
        catch (ApiException ex) when (ex.ResponseStatusCode == (int)HttpStatusCode.NotFound)
        {
            logger.LogWarning("Task with ID {TaskId} not found, redirecting to home", TaskId);
            navManager.NavigateTo("/");
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error fetching task with ID {TaskId}", TaskId);
            ApiError = true;
        }
    }

    private async Task GetInitiatives()
    {
        List<GetInitiativesInProjectResponse>? resp = await apiClient.Project[_projectId].Initiatives.GetAsync();

        if (resp is null)
        {
            return;
        }

        _initiatives = [.. resp.Select(x => new DropDownModel
        {
            DisplayStr = x.Name,
            Id = x.Id
        })];
    }

    private async Task OnSelectedInitiativeChanged(Guid? selectedInitiative)
    {
        if (selectedInitiative.HasValue)
        {
            _selectedInitiative = selectedInitiative.Value;
            await GetTaskQueues(_selectedInitiative.Value);
        }
    }

    private async Task GetTaskQueues(Guid initiativeId)
    {
        List<GetTaskQueuesInInitiativeResponse>? resp = await apiClient.Initiative[initiativeId].TaskQueues.GetAsync();

        if (resp is null)
        {
            return;
        }

        _taskQueues = [.. resp.Select(x => new DropDownModel
        {
            DisplayStr = x.TaskQueueName,
            Id = x.TaskQueueId
        }).Where(x => x.Id != _queueId)];
    }

    private async Task OnSelectedTaskQueueChanged(Guid? selectedQueue)
    {
        if (selectedQueue.HasValue)
        {
            _selectedNewTaskQueue = selectedQueue.Value;
            await Task.CompletedTask;
        }
    }

    private async Task SendMoveToQueueRequestAsync()
    {
        try
        {
            ApiError = false;

            if (!_selectedNewTaskQueue.HasValue)
            {
                return;
            }

            MoveTaskToQueueAction request = new()
            {
                TaskId = TaskId,
                TaskQueueId = _selectedNewTaskQueue.Value,
                ActionDateTime = DateTime.Now
            };

            await apiClient.Task.Action.MoveQueue.PostAsync(request);

            _moveToQueue = false;

            await FetchFromAPI();
        }
        catch (ApiException ex)
        {
            logger.LogError(ex, "Error while moving task to queue");
            ApiError = true;
        }
    }
}
