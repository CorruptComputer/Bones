using Bones.Shared.Consts;
using Bones.WebUI.Infrastructure;
using MudBlazor;
using MudExtensions;

namespace Bones.WebUI.Layout;

/// <summary>
///   The main layout of the application
/// </summary>
public partial class MainLayout(BonesAuthenticationStateProvider AuthStateProvider, ILogger<MainLayout> Logger, BonesApiClient ApiClient, NavigationManager NavManager) : LayoutComponentBase
{
    private MudTheme? _theme = null;

    private MudTheme Theme
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

    private MudSelectExtended<ProjectDropDownModel> _projectSelect = new();

    private bool _open = false;

    private bool _login = true;

    private ICollection<ProjectDropDownModel> Projects { get; set; } = [
        new()
        {
            ProjectName = "(loading)",
            ProjectId = null
        }
    ];

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        await UpdateProjectList();

        await base.OnInitializedAsync();
    }

    /// <inheritdoc />
    protected override async Task OnParametersSetAsync()
    {
        await UpdateProjectList();

        await base.OnParametersSetAsync();
    }

    private async Task LogoutAsync()
    {
        await ApiClient.LogoutAsync();
        await AuthStateProvider.ClearCurrentUserInBrowserStorageAsync(CancellationToken.None);

        NavManager.NavigateTo("/");
    }

    private async Task UpdateProjectList()
    {
        // Only load the project list if the user is authenticated
        if ((await AuthStateProvider.GetAuthenticationStateAsync()).User.Identity?.IsAuthenticated != true)
        {
            return;
        }

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

    private void ToggleDrawer()
    {
        _open = !_open;
    }

    private void ToggleLoginRegister()
    {
        _login = !_login;
    }

    private void OnGoToProjectChanged(IEnumerable<ProjectDropDownModel>? selectedProject)
    {
        Guid? selected = selectedProject?.FirstOrDefault()?.ProjectId;
        if (selected.HasValue)
        {
            if (selected.Value == Guid.Empty)
            {
                NavManager.NavigateTo(FrontEndUrls.Project.CREATE);
            }
            else
            {
                NavManager.NavigateTo(FrontEndUrls.Project.PROJECT_DASHBOARD.Replace(FrontEndUrls.Project.PROJECT_ID_PLACEHOLDER, selected.Value.ToString()));
            }
            
            _projectSelect.SelectOption(null);
        }
    }

    private sealed record ProjectDropDownModel
    {
        public required string ProjectName { get; init; }

        public required Guid? ProjectId { get; init; }

        public override string ToString()
        {
            return ProjectName;
        }

        public static implicit operator string(ProjectDropDownModel model)
        {
            return model.ToString();
        }
    }
}