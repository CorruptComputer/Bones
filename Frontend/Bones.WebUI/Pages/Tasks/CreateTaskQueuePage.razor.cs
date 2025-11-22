using ReQuesty.Runtime.Abstractions;

namespace Bones.WebUI.Pages.Tasks;

/// <summary>
///   Page to create a task queue in an initiative
/// </summary>
public partial class CreateTaskQueuePage(BonesApiClient apiClient, NavigationManager navManager, ILogger<CreateTaskQueuePage> logger) : ComponentBase
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

    private string TaskQueueName { get; set; } = string.Empty;

    /// <summary>
    ///   Send the request to register to the API, if it errors tell the user what went wrong.
    /// </summary>
    public async Task SendCreateRequestAsync()
    {
        try
        {
            ApiError = false;

            Guid? queueId = await apiClient.TaskQueue.CreateInInitiative.PostAsync(new()
            {
                Name = TaskQueueName,
                InitiativeId = InitiativeId
            });

            if (queueId is null)
            {
                ApiError = true;
                return;
            }

            navManager.NavigateTo(FrontEndUrls.Task.TASK_QUEUE_DASHBOARD.Replace(FrontEndUrls.Task.TASK_QUEUE_ID_PLACEHOLDER, queueId.ToString()));
        }
        catch (ApiException ex)
        {
            logger.LogError(ex, "Error while creating the task queue");
            ApiError = true;
        }
    }
}