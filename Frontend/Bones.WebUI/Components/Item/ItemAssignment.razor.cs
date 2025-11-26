using System.Net;
using ReQuesty.Runtime.Abstractions;

namespace Bones.WebUI.Components.Item;

/// <summary>
///   Page for viewing an items assignees
/// </summary>
public partial class ItemAssignment(BonesApiClient apiClient, ILogger<ItemAssignment> logger) : ComponentBase
{
    /// <summary>
    ///   The ID of the asset to load
    /// </summary>
    [Parameter]
    public required Guid? AssetId { get; set; }

    /// <summary>
    ///   The ID of the task to load
    /// </summary>
    [Parameter]
    public required Guid? TaskId { get; set; }

    private bool _apiError { get; set; } = false;

    private List<AssignmentSlotModel> _assignmentSlots = [];

    private List<AssigneeModel> _assignees = [];

    private Dictionary<AssignmentSlotModel, List<AssigneeModel>> _slotAssignees =>
        _assignmentSlots.ToDictionary(
            slot => slot,
            slot => _assignees.Where(a => a.AssignmentSlotId == slot.AssignmentSlotId).ToList()
        );

    /// <summary>
    ///   Fires when the component is loaded
    /// </summary>
    /// <returns></returns>
    protected override async Task OnInitializedAsync()
    {
        await FetchAssigneesFromAPI();

        await base.OnInitializedAsync();
    }

    private async Task FetchAssigneesFromAPI()
    {
        _apiError = false;

        try
        {
            GetLatestAssignmentsResponse? response = null;

            if (TaskId.HasValue)
            {
                response = await apiClient.Assignment.Task[TaskId.Value].Latest.GetAsync();
            }
            else if (AssetId.HasValue)
            {
                response = await apiClient.Assignment.Asset[AssetId.Value].Latest.GetAsync();
            }

            if (response is not null)
            {
                _assignmentSlots = response.AssignmentSlots ?? [];
                _assignees = response.Assignees ?? [];
            }
        }
        catch (ApiException ex) when (ex.ResponseStatusCode == (int)HttpStatusCode.NotFound)
        {
            logger.LogWarning("Task with ID {TaskId} not found", TaskId ?? AssetId);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error fetching task assignees with ID {TaskId}", TaskId ?? AssetId);
            _apiError = true;
        }
    }
}
