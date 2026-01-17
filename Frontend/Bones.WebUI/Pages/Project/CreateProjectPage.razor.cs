using ReQuesty.Runtime.Abstractions;

namespace Bones.WebUI.Pages.Project;

/// <summary>
///   Create project page
/// </summary>
public partial class CreateProjectPage(BonesApiClient apiClient, NavigationManager navManager, ILogger<CreateProjectPage> logger) : ComponentBase
{
    /// <summary>
    ///   Did the request to the API result in an error?
    /// </summary>
    public bool ApiError { get; set; } = false;

    /// <summary>
    ///   Is the form valid?
    /// </summary>
    public bool FormValid { get; set; }

    /// <summary>
    ///   The issues with the users inputs
    /// </summary>
    public string[] ValidationErrors { get; set; } = [];

    private string ProjectName { get; set; } = string.Empty;

    private ProjectPreset? projectPreset = null;

    /// <summary>
    ///   Send the request to register to the API, if it errors tell the user what went wrong.
    /// </summary>
    public async Task SendCreateRequestAsync()
    {
        try
        {
            ApiError = false;

            Guid? projectId = await apiClient.Project.Create.PostAsync(new()
            {
                Name = ProjectName,
                Preset = projectPreset,
                OrganizationId = null
            });

            if (projectId is null)
            {
                ApiError = true;
                logger.LogError("Project creation returned null project ID");
                return;
            }

            navManager.NavigateTo(FrontEndUrls.Project.PROJECT_DASHBOARD.Replace(FrontEndUrls.Project.PROJECT_ID_PLACEHOLDER, projectId.ToString()));
        }
        catch (ApiException ex)
        {
            logger.LogError(ex, "Error while creating project");
            ApiError = true;
        }
    }
}