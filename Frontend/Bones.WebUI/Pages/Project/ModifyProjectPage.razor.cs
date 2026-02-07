using System.ComponentModel.DataAnnotations;

namespace Bones.WebUI.Pages.Project;

/// <summary>
///   Modify a project page
/// </summary>
/// <param name="apiClient"></param>
public partial class ModifyProjectPage(BonesApiClient apiClient) : ComponentBase
{
    /// <summary>
    ///   The ID of the project to load
    /// </summary>
    [Required]
    [Parameter]
    public required Guid ProjectId { get; set; }

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
        GetProjectSettingsResponse? settingsResponse = await apiClient.Project[ProjectId].Settings.GetAsync();
        if (settingsResponse == null)
        {
            //ApiError = true;
            return;
        }

        ProjectName = settingsResponse.ProjectName;
        OwnerType = settingsResponse.OwnerType;
        OwnerId = settingsResponse.OwnerId;
        OwnerName = settingsResponse.OwnerDisplayName;
    }
}