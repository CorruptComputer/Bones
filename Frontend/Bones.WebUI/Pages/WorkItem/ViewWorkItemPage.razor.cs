using System.Globalization;
using System.Net;
using Bones.WebUI.Models;
using ReQuesty.Runtime.Abstractions;

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

    private int _currentVersion = 0;
    private Guid _projectId = Guid.Empty;
    private Guid _queueId = Guid.Empty;
    private string _queueName = string.Empty;
    private string _addedToQueueDateTime = string.Empty;
    private bool _moveToQueue = false;
    private string _workItemTitle { get; set; } = string.Empty;

    private List<DropDownModel> _initiatives { get; set; } = [
        new()
        {
            DisplayStr = "(loading)",
            Id = null
        }];

    private Guid? _selectedInitiative { get; set; }

    private List<DropDownModel> _workItemQueues { get; set; } = [
        new()
        {
            DisplayStr = "(loading)",
            Id = null
        }];

    private Guid? _selectedNewWorkItemQueue { get; set; }

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
            GetWorkItemByIdResponse? workItemResponse = await apiClient.WorkItem[WorkItemId].GetAsync();

            if (workItemResponse is not null)
            {
                _projectId = workItemResponse.ProjectId;
                _queueId = workItemResponse.WorkItemQueueId;
                _queueName = workItemResponse.WorkItemQueueName;
                _addedToQueueDateTime = workItemResponse.AddedToQueueDateTime.ToLocalTime().ToString(CultureInfo.CurrentCulture);
                _currentVersion = workItemResponse.CurrentVersion;
                _workItemTitle = workItemResponse.Title;

                await GetInitiatives();
            }
        }
        catch (ApiException ex) when (ex.ResponseStatusCode == (int)HttpStatusCode.NotFound)
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
            await GetWorkItemQueues(_selectedInitiative.Value);
        }
    }

    private async Task GetWorkItemQueues(Guid initiativeId)
    {
        List<GetWorkItemQueuesInInitiativeResponse>? resp = await apiClient.Initiative[initiativeId].WorkItemQueues.GetAsync();

        if (resp is null)
        {
            return;
        }

        _workItemQueues = [.. resp.Select(x => new DropDownModel
        {
            DisplayStr = x.QueueName,
            Id = x.QueueId
        }).Where(x => x.Id != _queueId)];
    }

    private async Task OnSelectedWorkItemQueueChanged(Guid? selectedQueue)
    {
        if (selectedQueue.HasValue)
        {
            _selectedNewWorkItemQueue = selectedQueue.Value;
            await Task.CompletedTask;
        }
    }

    private async Task SendMoveToQueueRequestAsync()
    {
        try
        {
            ApiError = false;

            if (!_selectedNewWorkItemQueue.HasValue)
            {
                return;
            }

            MoveWorkItemToQueueAction request = new()
            {
                WorkItemId = WorkItemId,
                WorkItemQueueId = _selectedNewWorkItemQueue.Value,
                ActionDateTime = DateTime.Now
            };

            await apiClient.WorkItem.Action.MoveQueue.PostAsync(request);

            _moveToQueue = false;

            await FetchFromAPI();
        }
        catch (ApiException ex)
        {
            logger.LogError(ex, "Error while moving work item to queue");
            ApiError = true;
        }
    }
}
