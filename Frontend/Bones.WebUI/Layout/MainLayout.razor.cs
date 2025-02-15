using Bones.Api.Client;
using Bones.Shared.Consts;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Bones.WebUI.Layout;

/// <summary>
/// 
/// </summary>
public partial class MainLayout : LayoutComponentBase
{
    private MudTheme? _theme = null;

    /// <summary>
    ///   The theme this app is going to use
    /// </summary>
    protected MudTheme Theme
    {
        get
        {
            if (_theme == null)
            {
                // TODO: Customize theme here
                _theme = new();
            }

            return _theme;
        }
    }

    private bool _open = false;

    private sealed record ProjectDropDownModel
    {
        public required string ProjectName { get; init; }

        public required Guid? ProjectId { get; init; }

        public override string ToString()
        {
            return ProjectName;
        }
    }

    private List<ProjectDropDownModel> Projects { get; set; } = [new()
    {
        ProjectName = "(loading)",
        ProjectId = null
    }];

    /// <summary>
    ///   Event for when the page is loaded
    /// </summary>
    protected override async Task OnInitializedAsync()
    {
        await UpdateProjectList();

        await base.OnInitializedAsync();
    }

    /// <summary>
    ///   Event for when the page is changed without a full site reload
    /// </summary>
    /// <returns></returns>
    protected override async Task OnParametersSetAsync()
    {
        await UpdateProjectList();

        await base.OnParametersSetAsync();
    }

    private async Task UpdateProjectList()
    {
        try
        {
            List<GetProjectQuickSelectResponse> projects = await ApiClient.GetProjectQuickSelectAsync();

            Projects = [];
            foreach (GetProjectQuickSelectResponse proj in projects)
            {
                Projects.Add(new()
                {
                    ProjectId = proj.ProjectId,
                    ProjectName = proj.ProjectName
                });
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, ex.Message);
        }

        Projects.Add(new()
        {
            ProjectName = "+ Create a new project",
            ProjectId = Guid.Empty
        });
    }

    /// <summary>
    ///   Toggles the nav drawer
    /// </summary>
    protected void ToggleDrawer()
    {
        _open = !_open;
    }

    /// <summary>
    ///    Navigates to the selected project
    /// </summary>
    /// <param name="selectedProject"></param>
    protected void OnGoToProjectChanged(IEnumerable<Guid?>? selectedProject)
    {
        Guid? selected = selectedProject?.FirstOrDefault();
        if (selected.HasValue)
        {
            if (selected.Value == Guid.Empty)
            {
                NavManager.NavigateTo(FrontEndUrls.Project.CREATE);
            }
            else
            {
                NavManager.NavigateTo(FrontEndUrls.Project.PROJECT_DASHBOARD.Replace("{ProjectId:guid}", selected.Value.ToString()));
            }
        }
    }
}