using Bones.Api.Client;
using Microsoft.AspNetCore.Components;

namespace Bones.WebUI.Pages.Project;

/// <summary>
///   Dashboard for projects
/// </summary>
public partial class ProjectDashboardPage : ComponentBase
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
    ///   The number of initiatives on the project, received from the API
    /// </summary>
    public int? InitiativeCount { get; set; }

    /// <summary>
    ///   The type of owner for the project, received from the API
    /// </summary>
    public OwnershipType? OwnerType { get; set; }

    /// <summary>
    ///   The ID of the owner of the project, received from the API
    /// </summary>
    public Guid? OwnerId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    protected override async Task OnInitializedAsync()
    {
        GetProjectDashboardResponse dashboardResponse = await ApiClient.GetProjectDashboardAsync(ProjectId);
        ProjectName = dashboardResponse.ProjectName;
        InitiativeCount = dashboardResponse.InitiativeCount;
        OwnerType = dashboardResponse.OwnerType;
        OwnerId = dashboardResponse.OwnerId;
        // @page "/Project/{ProjectId:guid}/Dashboard/"
        // 
        await base.OnInitializedAsync();
    }
}