using System.Text.Json;
using Bones.Shared.Consts;

namespace Bones.WebUI.Pages.WorkItem;

/// <summary>
///   Page for creating a new work item
/// </summary>
public partial class CreateWorkItemPage(BonesApiClient apiClient, NavigationManager navManager, ILogger<CreateWorkItemPage> logger) : ComponentBase
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
            Name = "(loading)",
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
            Name = "(loading)",
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
            Name = "(loading)",
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
            Name = "(loading)",
            Id = null
        }];

    /// <summary>
    ///   The work item layout selected by the user
    /// </summary>
    protected Guid? SelectedWorkItemLayout { get; set; }

    private IOrderedEnumerable<GetLatestItemLayoutVersionFieldsResponse> _currentLayoutVersionFields = Enumerable.Empty<GetLatestItemLayoutVersionFieldsResponse>().OrderBy(x => x.OrderNumber);

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
            Name = x.ProjectName,
            Id = x.ProjectId
        })];
    }

    private async Task GetInitiatives(Guid projectId)
    {
        List<GetInitiativesInProjectResponse> resp = await apiClient.GetInitiativesInProjectAsync(projectId);

        Initiatives = [.. resp.Select(x => new DropDownModel
        {
            Name = x.Name,
            Id = x.Id
        })];
    }

    private async Task GetWorkItemQueues(Guid initiativeId)
    {
        List<GetWorkItemQueuesInInitiativeResponse> resp = await apiClient.GetWorkItemQueuesInInitiativeAsync(initiativeId);

        WorkItemQueues = [.. resp.Select(x => new DropDownModel
        {
            Name = x.QueueName,
            Id = x.QueueId
        })];
    }

    private async Task GetWorkItemLayouts()
    {
        if (!SelectedProject.HasValue)
        {
            return;
        }

        List<GetProjectLayoutsResponse> resp = await apiClient.GetProjectLayoutsAsync(SelectedProject.Value, ItemLayoutUses.WorkItems);

        WorkItemLayouts = [.. resp.Select(x => new DropDownModel
        {
            Name = $"{x.FriendlyIdPrefix} - {x.LayoutName}",
            Id = x.LayoutId
        })];
    }

    private async Task GetCurrentLayoutVersionFields()
    {
        if (!SelectedWorkItemLayout.HasValue)
        {
            return;
        }

        List<GetLatestItemLayoutVersionFieldsResponse> resp = await apiClient.GetLatestItemLayoutVersionFieldsAsync(SelectedWorkItemLayout.Value);

        _currentLayoutVersionFields = resp.OrderBy(x => x.OrderNumber);

        _boolValues = [];
        _dateTimeValues = [];
        _decimalValues = [];
        _integerValues = [];
        _stringValues = [];

        foreach (GetLatestItemLayoutVersionFieldsResponse field in _currentLayoutVersionFields)
        {
            switch (field.Type)
            {
                case FieldType.TextField or FieldType.TextBox or FieldType.ValueList:
                    _stringValues[field.Id] = string.Empty;
                    break;
                case FieldType.Integer:
                    _integerValues[field.Id] = null;
                    break;
                case FieldType.Decimal:
                    _decimalValues[field.Id] = null;
                    break;
                case FieldType.Boolean:
                    _boolValues[field.Id] = field.IsRequired ? false : null;
                    break;
                case FieldType.DateTime:
                    _dateTimeValues[field.Id] = null;
                    _timeSpanValues[field.Id] = null;
                    break;
                //FieldType.GeoLocation => null, // TODO: Implement
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
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
            await GetCurrentLayoutVersionFields();
        }
    }

    private async Task SendCreateRequestAsync()
    {
        if (!FormValid)
        {
            return;
        }

        try
        {
            ApiError = false;

            if (!SelectedWorkItemQueue.HasValue || !SelectedWorkItemLayout.HasValue)
            {
                return;
            }

            List<ItemValueModel> values = [];
            foreach (KeyValuePair<Guid, bool?> field in _boolValues)
            {
                if (field.Value.HasValue)
                {
                    values.Add(new()
                    {
                        FieldVersionId = field.Key,
                        BoolValue = field.Value
                    });
                }
            }

            foreach (KeyValuePair<Guid, DateTime?> field in _dateTimeValues)
            {
                TimeSpan? timeSpan = _timeSpanValues[field.Key];
                
                if (field.Value.HasValue && timeSpan.HasValue)
                {
                    values.Add(new()
                    {
                        FieldVersionId = field.Key,
                        DateTimeValue = field.Value.Value.Add(timeSpan.Value)
                    });
                }
            }

            foreach (KeyValuePair<Guid, double?> field in _decimalValues)
            {
                if (field.Value.HasValue)
                {
                    values.Add(new()
                    {
                        FieldVersionId = field.Key,
                        DecimalValue = field.Value
                    });
                }
            }

            foreach (KeyValuePair<Guid, long?> field in _integerValues)
            {
                if (field.Value.HasValue)
                {
                    values.Add(new()
                    {
                        FieldVersionId = field.Key,
                        IntValue = field.Value
                    });
                }
            }

            foreach (KeyValuePair<Guid, string> field in _stringValues)
            {
                if (!string.IsNullOrWhiteSpace(field.Value))
                {
                    values.Add(new()
                    {
                        FieldVersionId = field.Key,
                        StrValue = field.Value
                    });
                }
            }

            CreateWorkItemRequest request = new()
            {
                Name = "Test",
                WorkItemLayoutId = SelectedWorkItemLayout.Value,
                FieldValues = values
            };

            logger.LogWarning("Creating work item with request: {@Request}", JsonSerializer.Serialize(request));

            await apiClient.CreateWorkItemInQueueAsync(SelectedWorkItemQueue.Value, request);
            
            navManager.NavigateTo(FrontEndUrls.WorkItem.WORKITEM_QUEUE_DASHBOARD.Replace(FrontEndUrls.WorkItem.WORKITEM_QUEUE_ID_PLACEHOLDER, SelectedWorkItemQueue.Value.ToString()));
        }
        catch (ApiException ex)
        {
            logger.LogError(ex, "Error while creating a work item");
            ApiError = true;
        }
    }

    /// <summary>
    ///   The model for the drop down menus
    /// </summary>
    protected sealed record DropDownModel
    {
        /// <summary>
        ///   The name of the option
        /// </summary>
        public required string Name { get; init; }

        /// <summary>
        ///   The ID of the option
        /// </summary>
        public required Guid? Id { get; init; }

        /// <summary>
        ///   The string representation of the option, which is just the name
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }
    }
}
