namespace Bones.WebUI.Components.Project.Modify;

/// <summary>
///   Component for viewing a project's item assignments
/// </summary>
public partial class ProjectAssignmentsSummary(BonesApiClient apiClient, ILogger<ProjectAssignmentsSummary> logger) : ComponentBase
{
    /// <summary>
    ///   The ID of the project to load
    /// </summary>
    [Parameter, EditorRequired]
    public required Guid ProjectId { get; set; }

    /// <summary>
    ///   The number of item assignments on the project, received from the API
    /// </summary>
    public int? ItemAssignmentsCount { get; set; }

    /// <summary>
    ///   Is the item assignments list still loading?
    /// </summary>
    public bool ItemAssignmentsListLoading { get; set; } = true;

    // <summary>
    //   The list of item assignments on the project, received from the API
    // </summary>
    //public List<ItemAssignmentModel> ItemAssignmentsList { get; set; } = [];

    /// <summary>
    ///   Fires when the component is loaded
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
        // TODO: Replace with different API call
        GetProjectSettingsResponse? settingsResponse = await apiClient.Project[ProjectId].Settings.GetAsync();
        if (settingsResponse == null)
        {
            logger.LogError("Failed to fetch project assignments for project {ProjectId}", ProjectId);
            return;
        }

        //ItemAssignmentsCount = settingsResponse.ItemAssignmentsCount;
        //ItemAssignmentsList = settingsResponse.ItemAssignments;
        //ItemAssignmentsListLoading = false;
    }

    // <summary>
    //   Gets the URL to create a new item assignment
    // </summary>
    // <param name="projectId"></param>
    // <returns></returns>
    //protected static string GetCreateItemAssignmentUrl(Guid projectId) => FrontEndUrls.Project.ItemAssignment.ITEM_ASSIGNMENT
    //    .Replace(FrontEndUrls.Project.PROJECT_ID_PLACEHOLDER, projectId.ToString());

    // <summary>
    //   Gets the URL to edit an item assignment
    // </summary>
    // <param name="projectId"></param>
    // <param name="assignmentId"></param>
    // <returns></returns>
    //protected static string GetEditItemAssignmentUrl(Guid projectId, Guid assignmentId) => FrontEndUrls.Project.ItemAssignment.ITEM_ASSIGNMENT_WITH_ID
    //    .Replace(FrontEndUrls.Project.PROJECT_ID_PLACEHOLDER, projectId.ToString())
    //    .Replace(FrontEndUrls.Project.ItemAssignment.ITEM_ASSIGNMENT_ID_PLACEHOLDER, assignmentId.ToString());
}