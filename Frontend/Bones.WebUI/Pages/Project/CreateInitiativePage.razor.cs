using Bones.Shared.Consts;
using MudBlazor;

namespace Bones.WebUI.Pages.Project;

/// <summary>
///   Create Initiative page
/// </summary>
public partial class CreateInitiativePage(BonesApiClient ApiClient, NavigationManager NavManager, ILogger<CreateInitiativePage> Logger) : ComponentBase
{
    /// <summary>
    ///   The ID of the project to load in this dashboard
    /// </summary>
    [Parameter]
    public Guid ProjectId { get; set; }

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

    private MudTextField<string> InitiativeName { get; set; } = new();

    /// <summary>
    ///   Send the request to register to the API, if it errors tell the user what went wrong.
    /// </summary>
    public async Task SendCreateRequestAsync()
    {
        try
        {
            ApiError = false;

            Guid initiativeId = await ApiClient.CreateInitiativeAsync(ProjectId, new()
            {
                Name = InitiativeName.Text
            });

            NavManager.NavigateTo(FrontEndUrls.Project.Initiative.INITIATIVE_DASHBOARD.Replace(FrontEndUrls.Project.PROJECT_ID_PLACEHOLDER, ProjectId.ToString()).Replace("{InitiativeId:guid}", initiativeId.ToString()));
        }
        catch (ApiException ex)
        {
            Logger.LogError(ex, "Error while creating initiative");
            ApiError = true;
        }
    }
}