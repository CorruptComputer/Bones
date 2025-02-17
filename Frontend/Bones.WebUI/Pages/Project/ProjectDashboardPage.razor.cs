using Bones.Api.Client;
using Microsoft.AspNetCore.Components;

namespace Bones.WebUI.Pages.Project;

/// <summary>
///   Dashboard for projects
/// </summary>
public partial class ProjectDashboardPage(BonesApiClient ApiClient) : ComponentBase
{
    /// <summary>
    ///   The ID of the project to load in this dashboard
    /// </summary>
    [Parameter]
    public Guid ProjectId { get; set; }

    /// <summary>
    ///   The name of the project, received from the API
    /// </summary>
    public string? ProjectName { get; set; }

    /// <summary>
    ///   The type of owner for the project, received from the API
    /// </summary>
    public OwnershipType? OwnerType { get; set; }

    /// <summary>
    ///   The ID of the owner of the project, received from the API
    /// </summary>
    public Guid? OwnerId { get; set; }

    /// <summary>
    ///   The name of the owner of the project, received from the API
    /// </summary>
    public string? OwnerName { get; set; }

    /// <summary>
    ///   The number of initiatives on the project, received from the API
    /// </summary>
    public int? InitiativeCount { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool InitiativeListLoading { get; set; } = true;

    /// <summary>
    /// 
    /// </summary>
    public List<InitiativeListModel> InitiativeList { get; set; } = [];

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
        GetProjectDashboardResponse dashboardResponse = await ApiClient.GetProjectDashboardAsync(ProjectId);
        ProjectName = dashboardResponse.ProjectName;
        OwnerType = dashboardResponse.OwnerType;
        OwnerId = dashboardResponse.OwnerId;
        OwnerName = dashboardResponse.OwnerDisplayName;

        InitiativeCount = dashboardResponse.InitiativeCount;
        InitiativeList = dashboardResponse.Initiatives;
        InitiativeListLoading = false;
    }
}