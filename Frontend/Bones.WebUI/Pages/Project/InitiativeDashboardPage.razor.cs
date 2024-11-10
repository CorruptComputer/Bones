using Bones.Api.Client;
using Microsoft.AspNetCore.Components;

namespace Bones.WebUI.Pages.Project;

/// <summary>
///   Dashboard for projects
/// </summary>
public partial class InitiativeDashboardPage : ComponentBase
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
    ///   The name of the project, received from the API
    /// </summary>
    public string? InitiativeName { get; set; }

    /// <summary>
    ///   The number of initiatives on the project, received from the API
    /// </summary>
    public int? QueueCount { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool QueueListLoading { get; set; } = true;

    /// <summary>
    /// 
    /// </summary>
    public List<InitiativeListModel> QueueList { get; set; } = [];

    /// <summary>
    /// 
    /// </summary>
    protected override async Task OnInitializedAsync()
    {
        // TODO: Garbage data for now, do real later
        GetProjectDashboardResponse dashboardResponse = await ApiClient.GetProjectDashboardAsync(ProjectId);
        InitiativeName = dashboardResponse.ProjectName;

        QueueCount = dashboardResponse.InitiativeCount;
        QueueList = dashboardResponse.Initiatives;
        QueueListLoading = false;

        await base.OnInitializedAsync();
    }
}