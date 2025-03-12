using Bones.Shared.Consts;
using MudBlazor;

namespace Bones.WebUI.Pages.WorkItem;

/// <summary>
///   Page to create a work item queue in an initiative
/// </summary>                                           
public partial class CreateWorkItemQueuePage(BonesApiClient apiClient, NavigationManager navManager, ILogger<CreateWorkItemQueuePage> logger) : ComponentBase
{
    /// <summary>
    ///   The ID of the initiative
    /// </summary>
    [Parameter]
    public Guid InitiativeId { get; set; }

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

    private MudTextField<string> WorkItemQueueName { get; set; } = new();

    /// <summary>
    ///   Send the request to register to the API, if it errors tell the user what went wrong.
    /// </summary>
    public async Task SendCreateRequestAsync()
    {
        try
        {
            ApiError = false;

            await Task.CompletedTask;

            await apiClient.CreateQueueInInitiativeAsync(InitiativeId, new()
            {
                Name = WorkItemQueueName.Text
            });

            navManager.NavigateTo(FrontEndUrls.Project.Initiative.INITIATIVE_DASHBOARD.Replace(FrontEndUrls.Project.Initiative.INITIATIVE_ID_PLACEHOLDER, InitiativeId.ToString()));
        }
        catch (ApiException ex)
        {
            logger.LogError(ex, "Error while creating the work item queue");
            ApiError = true;
        }
    }
}