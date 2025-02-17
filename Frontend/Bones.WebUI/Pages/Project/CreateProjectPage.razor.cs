using Bones.Api.Client;
using Bones.Shared.Consts;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Bones.WebUI.Pages.Project;

/// <summary>
///   Create project page
/// </summary>
public partial class CreateProjectPage(BonesApiClient ApiClient, NavigationManager NavManager, ILogger<CreateProjectPage> Logger) : ComponentBase
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

    private MudTextField<string> ProjectName { get; set; } = new();

    /// <summary>
    ///   Send the request to register to the API, if it errors tell the user what went wrong.
    /// </summary>
    public async Task SendCreateRequestAsync()
    {
        try
        {
            ApiError = false;

            Guid projectId = await ApiClient.CreateProjectAsync(new()
            {
                Name = ProjectName.Text,
                OrganizationId = null
            });

            NavManager.NavigateTo(FrontEndUrls.Project.PROJECT_DASHBOARD.Replace("{ProjectId:guid}", projectId.ToString()));
        }
        catch (ApiException ex)
        {
            Logger.LogError(ex, "Error while creating project");
            ApiError = true;
        }
    }
}