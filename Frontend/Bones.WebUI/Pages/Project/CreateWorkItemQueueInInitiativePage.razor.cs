using Bones.Api.Client;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Bones.WebUI.Pages.Project;

/// <summary>
///   Page to create a work item queue in an initiative
/// </summary>                                          BonesApiClient ApiClient, NavigationManager NavManager, 
public partial class CreateWorkItemQueueInInitiativePage(ILogger<CreateWorkItemQueueInInitiativePage> Logger) : ComponentBase
{
    /// <summary>
    ///   The ID of the project
    /// </summary>
    [Parameter]
    public Guid ProjectId { get; set; }

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

            //Guid workItemQueueId = await ApiClient.CreateWorkItemQueueAsync(InitiativeId, new()
            //{
            //    Name = WorkItemQueueName.Text
            //});

            //NavManager.NavigateTo(FrontEndUrls.Project.Initiative.INITIATIVE_DASHBOARD.Replace("{ProjectId:guid}", InitiativeId.ToString()).Replace("{InitiativeId:guid}", initiativeId.ToString()));
        }
        catch (ApiException ex)
        {
            Logger.LogError(ex, "Error while creating the work item queue");
            ApiError = true;
        }
    }
}