using System.Globalization;
using System.Net;
using Bones.Shared.Consts;
using Bones.Shared.Exceptions;
using Bones.WebUI.Models;

namespace Bones.WebUI.Components.GenericItem;

/// <summary>
///   Page for viewing a work item
/// </summary>
public partial class GenericItemEditor(BonesApiClient apiClient, NavigationManager navManager, ILogger<GenericItemEditor> logger) : ComponentBase
{
    /// <summary>
    ///   The ID of the item to load
    /// </summary>
    [Parameter]
    public required Guid? ItemId { get; set; }

    /// <summary>
    ///   The mode this editor should operate in
    /// </summary>
    [Parameter]
    public required Mode EditorMode { get; set; }

    /// <summary>
    ///   If the editor is in Create mode, this is the ID of the work item queue to create the item in
    /// </summary>
    [Parameter]
    public Guid? WorkItemQueueId { get; set; }

    /// <summary>
    ///   If the editor is in Create mode, this is the ID of the work item layout to use for the item
    /// </summary>
    [Parameter]
    public Guid? ItemLayoutId { get; set; }

    private bool _apiError { get; set; } = false;

    private bool _editFormValid { get; set; }

    private string[] _editFormValidationErrors { get; set; } = [];

    /// <summary>
    ///   Item values displayed on the page for this work item, if editing is enabled: the values that are being changed.
    /// </summary>
    private IOrderedEnumerable<WorkItemValueModel> _currentValues = Enumerable.Empty<WorkItemValueModel>().OrderBy(x => x.OrderNumber);

    /// <summary>
    ///   Do not update this after its been loaded from the API.
    /// </summary>
    private IOrderedEnumerable<WorkItemValueModel> _originalValues = Enumerable.Empty<WorkItemValueModel>().OrderBy(x => x.OrderNumber);

    private Guid _layoutId = Guid.Empty;
    private bool _editing = false;

    private string _workItemTitle { get; set; } = string.Empty;
    private Dictionary<Guid, bool?> _boolValues { get; set; } = [];
    private Dictionary<Guid, DateTime?> _dateTimeValues { get; set; } = [];
    private Dictionary<Guid, TimeSpan?> _timeSpanValues { get; set; } = [];
    private Dictionary<Guid, double?> _decimalValues { get; set; } = [];
    private Dictionary<Guid, long?> _integerValues { get; set; } = [];
    private Dictionary<Guid, string> _stringValues { get; set; } = [];

    /// <summary>
    ///   Fires when the component is loaded
    /// </summary>
    /// <returns></returns>
    protected override async Task OnInitializedAsync()
    {
        await FetchWorkItemFromAPI();

        if (EditorMode == Mode.Create)
        {
            await FetchLayoutFromAPI();
        }

        await base.OnInitializedAsync();
    }

    private async Task FetchLayoutFromAPI()
    {
        _apiError = false;

        if (ItemLayoutId is null)
        {
            return;
        }

        try
        {
            List<GetLatestItemLayoutVersionFieldsResponse>? layoutFields = await apiClient.GetLatestItemLayoutVersionFieldsAsync(ItemLayoutId.Value);
            _currentValues = layoutFields.Select(lf => new WorkItemValueModel
            {
                OrderNumber = lf.OrderNumber,
                FieldVersionId = lf.Id,
                Name = lf.Name,
                ValueType = lf.Type,
                IsRequired = lf.IsRequired,
                CanBeNegative = lf.CanBeNegative,
                PossibleValues = lf.PossibleValues,
            }).OrderBy(x => x.OrderNumber);

            foreach (WorkItemValueModel field in _currentValues)
            {
                switch (field.ValueType)
                {
                    case FieldType.TextField or FieldType.TextBox or FieldType.ValueList:
                        _stringValues[field.FieldVersionId] = string.Empty;
                        break;
                    case FieldType.Integer:
                        _integerValues[field.FieldVersionId] = null;
                        break;
                    case FieldType.Decimal:
                        _decimalValues[field.FieldVersionId] = null;
                        break;
                    case FieldType.Boolean:
                        _boolValues[field.FieldVersionId] = null;
                        break;
                    case FieldType.DateTime:
                        _dateTimeValues[field.FieldVersionId] = null;
                        _timeSpanValues[field.FieldVersionId] = null;
                        break;
                    //FieldType.GeoLocation => null, // TODO: Implement
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

        }
        catch (ApiException<ErrorResponse> ex) when (ex.StatusCode == (int)HttpStatusCode.NotFound)
        {
            logger.LogWarning("Item layout with ID {ItemLayoutId} not found, redirecting to home", ItemLayoutId);
            navManager.NavigateTo("/");
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error fetching item layout with ID {ItemLayoutId}", ItemLayoutId);
            _apiError = true;
        }
    }

    private async Task FetchWorkItemFromAPI()
    {
        _apiError = false;

        if (ItemId is null || ItemId == Guid.Empty)
        {
            return;
        }

        try
        {
            GetWorkItemByIdResponse? workItemResponse = await apiClient.GetWorkItemByIdAsync(ItemId.Value);

            if (workItemResponse is not null)
            {
                _originalValues = _currentValues = workItemResponse.ItemValues.OrderBy(x => x.OrderNumber);
                _workItemTitle = workItemResponse.Title;
                _layoutId = workItemResponse.LayoutId;

                foreach (WorkItemValueModel field in _originalValues)
                {
                    switch (field.ValueType)
                    {
                        case FieldType.TextField or FieldType.TextBox or FieldType.ValueList:
                            _stringValues[field.FieldVersionId] = field.Value ?? string.Empty;
                            break;
                        case FieldType.Integer:
                            _integerValues[field.FieldVersionId] = field.Value is null ? null : Convert.ToInt64(field.Value);
                            break;
                        case FieldType.Decimal:
                            _decimalValues[field.FieldVersionId] = field.Value is null ? null : Convert.ToDouble(field.Value);
                            break;
                        case FieldType.Boolean:
                            _boolValues[field.FieldVersionId] = field.Value is null ? null : Convert.ToBoolean(field.Value);
                            break;
                        case FieldType.DateTime:
                            _dateTimeValues[field.FieldVersionId] = field.Value is null ? null : DateTimeOffset.Parse(field.Value, CultureInfo.InvariantCulture).ToLocalTime().DateTime;
                            _timeSpanValues[field.FieldVersionId] = field.Value is null ? null : DateTimeOffset.Parse(field.Value, CultureInfo.InvariantCulture).ToLocalTime().TimeOfDay;
                            break;
                        //FieldType.GeoLocation => null, // TODO: Implement
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }
        catch (ApiException<ErrorResponse> ex) when (ex.StatusCode == (int)HttpStatusCode.NotFound)
        {
            logger.LogWarning("Work item with ID {WorkItemId} not found, redirecting to home", ItemId);
            navManager.NavigateTo("/");
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error fetching work item with ID {WorkItemId}", ItemId);
            _apiError = true;
        }
    }

    private async Task SendCreateRequestAsync()
    {
        if (!_editFormValid)
        {
            return;
        }

        try
        {
            _apiError = false;

            if (!WorkItemQueueId.HasValue || !ItemLayoutId.HasValue)
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

            CreateWorkItemAction request = new()
            {
                ActionDateTime = DateTime.Now,
                WorkItemLayoutId = ItemLayoutId.Value,
                WorkItemQueueId = WorkItemQueueId.Value,
                Title = _workItemTitle,
                FieldValues = values
            };

            await apiClient.CreateWorkItemActionAsync(request);

            navManager.NavigateTo(FrontEndUrls.WorkItem.WORKITEM_QUEUE_DASHBOARD.Replace(FrontEndUrls.WorkItem.WORKITEM_QUEUE_ID_PLACEHOLDER, WorkItemQueueId.Value.ToString()));
        }
        catch (ApiException ex)
        {
            logger.LogError(ex, "Error while creating a work item");
            _apiError = true;
        }
    }

    private async Task SendCreateVersionRequestAsync()
    {
        if (!_editFormValid)
        {
            return;
        }

        try
        {
            _apiError = false;

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

            CreateWorkItemVersionAction request = new()
            {
                WorkItemId = ItemId ?? throw new BonesException("Work item ID cannot be null"),
                WorkItemLayoutId = _layoutId,
                ActionDateTime = DateTime.Now,
                Title = _workItemTitle,
                FieldValues = values
            };

            await apiClient.CreateWorkItemVersionActionAsync(request);

            _editing = false;
        }
        catch (ApiException ex)
        {
            logger.LogError(ex, "Error while creating a work item");
            _apiError = true;
        }
    }

    /// <summary>
    ///   The mode for the editor
    /// </summary>
    public enum Mode
    {
        /// <summary>
        ///   Default view, editing is allowed
        /// </summary>
        ViewEdit,

        /// <summary>
        ///   View only
        /// </summary>
        View,

        /// <summary>
        ///   Edit only
        /// </summary>
        Edit,

        /// <summary>
        ///   Creation of a new item
        /// </summary>
        Create
    }
}
