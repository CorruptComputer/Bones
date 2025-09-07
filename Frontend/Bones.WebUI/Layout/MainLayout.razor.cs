using System.Net;
using System.Reflection;
using Bones.Shared.Consts;
using Bones.WebUI.Services.Singleton;
using MudBlazor;
using MudExtensions;
using ReQuesty.Runtime.Abstractions;

namespace Bones.WebUI.Layout;

/// <summary>
///   The main layout of the application
/// </summary>
public partial class MainLayout(BonesAuthenticationStateProvider authStateProvider, ILogger<MainLayout> logger,
    BonesApiClient apiClient, NavigationManager navManager, BonesConfigurationProvider configProvider) : LayoutComponentBase
{
    /// <summary>
    ///   The ID of the user this request is for
    /// </summary>
    [Parameter]
    [SupplyParameterFromQuery]
    public string? UserId { get; set; }

    /// <summary>
    ///   The code to validate the change/confirm email request
    /// </summary>
    [Parameter]
    [SupplyParameterFromQuery]
    public string? Code { get; set; }

    /// <summary>
    ///   If this was a request to change the users email, what is their new email?
    /// </summary>
    [Parameter]
    [SupplyParameterFromQuery]
    public string? ChangedEmail { get; set; }

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

    /// <summary>
    ///   The current state of the email confirmation
    /// </summary>
    public enum ConfirmEmailState
    {
        /// <summary>
        ///   ¯\_(ツ)_/¯
        /// </summary>
        Unknown,

        /// <summary>
        ///   Good to go
        /// </summary>
        Success,

        /// <summary>
        ///   Something went wrong
        /// </summary>
        Failure
    }

    /// <summary>
    ///   The current state of this page
    /// </summary>
    public ConfirmEmailState CurrentState { get; set; } = ConfirmEmailState.Unknown;

    private readonly string _webUiVersion = Assembly.GetEntryAssembly()
                                                    ?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                                                    // For example: 0.0.1+b9d1873a
                                                    ?.InformationalVersion.Split('+')[1] ?? "ERROR";

    private string _apiVersion = string.Empty;

    private MudSelectExtended<ProjectDropDownModel> _projectSelect = new();

    private bool _openDrawer = false;

    private bool _login = true;

    private bool _confirmEmail = false;

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

        // If these are provided, we are in the email confirmation flow
        // I don't think this needs to be added to the OnParametersSetMethod, we only need it once
        if (!string.IsNullOrEmpty(UserId)
            && Guid.TryParse(UserId, out Guid parsedUserId)
            && !string.IsNullOrEmpty(Code))
        {
            _confirmEmail = true;

            try
            {
                await apiClient.Anonymous.ConfirmEmail.GetAsync(req =>
                {
                    req.QueryParameters.UserId = parsedUserId;
                    req.QueryParameters.Code = Code;
                    req.QueryParameters.ChangedEmail = ChangedEmail;
                });

                CurrentState = ConfirmEmailState.Success;
            }
            catch (Exception)
            {
                CurrentState = ConfirmEmailState.Failure;
            }
        }

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
        await apiClient.Login.Logout.PostAsync();
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
            List<GetProjectQuickSelectResponse>? projects = await apiClient.Project.Projects.QuickSelect.GetAsync();

            if (projects is null)
            {
                throw new InvalidOperationException("Received null project list from API");
            }

            Projects = [.. projects.Select(proj => new ProjectDropDownModel
            {
                ProjectId = proj.ProjectId,
                ProjectName = proj.ProjectName
            })];
        }
        catch (ApiException ex) when (ex.ResponseStatusCode == (int)HttpStatusCode.Unauthorized)
        {
            // Skip doing anything else since we'll get redirected to the login page anyway
            return;
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
        StateHasChanged();
    }

    private void ToggleLoginRegister()
    {
        _login = !_login;
        StateHasChanged();
    }

    private void ReturnToLogin()
    {
        _login = true;
        _confirmEmail = false;
        UserId = null;
        Code = null;
        ChangedEmail = null;
        StateHasChanged();

        navManager.NavigateTo("/");
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