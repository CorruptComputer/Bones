using System.Reflection;
using Bones.Shared.Consts;
using Bones.WebUI.Infrastructure;
using MudBlazor;
using MudExtensions;

namespace Bones.WebUI.Layout;

/// <summary>
///   The main layout of the application
/// </summary>
public partial class MainLayout(BonesAuthenticationStateProvider authStateProvider, ILogger<MainLayout> logger,
    BonesApiClient apiClient, NavigationManager navManager, BonesConfigurationProvider configProvider) : LayoutComponentBase
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

    private readonly string _webUiVersion = Assembly.GetEntryAssembly()
                                                    ?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                                                    // For example: 0.0.1+b9d1873a
                                                    ?.InformationalVersion.Split('+')[1] ?? "ERROR";

    private string _apiVersion = string.Empty;

    private MudSelectExtended<ProjectDropDownModel> _projectSelect = new();

    private bool _openDrawer = false;

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
        _apiVersion = (await configProvider.GetApiConfigAsync(default)).ApiVersion;

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
        await apiClient.LogoutAsync();
        await authStateProvider.ClearCurrentUserInBrowserStorageAsync(CancellationToken.None);

        navManager.NavigateTo("/");
    }

    private async Task UpdateProjectList()
    {
        // Only load the project list if the user is authenticated
        if ((await authStateProvider.GetAuthenticationStateAsync()).User.Identity?.IsAuthenticated != true)
        {
            return;
        }

        await _projectSelect.Clear();

        try
        {
            List<GetProjectQuickSelectResponse> projects = await apiClient.GetProjectQuickSelectAsync();

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
            logger.LogError(ex, ex.Message);
        }

        Projects.Add(new()
        {
            ProjectName = "+ Create a new project",
            ProjectId = Guid.Empty
        });
    }

    private void ToggleDrawer()
    {
        _openDrawer = !_openDrawer;
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
                navManager.NavigateTo(FrontEndUrls.Project.CREATE);
            }
            else
            {
                navManager.NavigateTo(FrontEndUrls.Project.PROJECT_DASHBOARD.Replace(FrontEndUrls.Project.PROJECT_ID_PLACEHOLDER, selected.Value.ToString()));
            }
        }
    }

    private sealed record ProjectDropDownModel
    {
        public required string ProjectName { get; init; }

        /// <summary>
        ///   If null, the selection will be ignored.
        ///   If Guid.Empty, the user will be redirected to the create project page.
        ///   If a valid Guid, the user will be redirected to the project dashboard.
        /// </summary>
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