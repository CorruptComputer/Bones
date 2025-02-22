namespace Bones.WebUI.Components.SystemAdmin.SystemSettings;

/// <summary>
///   Component for viewing and updating the web UI base URL
/// </summary>
/// <param name="apiClient"></param>
public partial class WebUiBaseUrlComponent(BonesApiClient apiClient) : ComponentBase
{
    /// <summary>
    ///   Is the form valid?
    /// </summary>
    protected bool FormValid { get; set; }

    /// <summary>
    ///   The issues with the users input
    /// </summary>
    protected string[] ValidationErrors { get; set; } = [];

    /// <summary>
    ///   The base url for the web UI
    /// </summary>
    public string? BaseUrl { get; set; }

    /// <summary>
    ///   If updating, the reason for the change
    /// </summary>
    public string? ChangeReason { get; set; }

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
        string baseUrl = await apiClient.GetWebUiBaseUrlAsync();
        BaseUrl = baseUrl;
    }

    /// <summary>
    ///   
    /// </summary>
    /// <returns></returns>
    protected async Task Update()
    {
        try
        {
            await apiClient.SaveWebUiBaseUrlAsync(new()
            {
                BaseUrl = BaseUrl,
                ChangeReason = ChangeReason
            });
        }
        catch (ApiException e)
        {
            ValidationErrors = [e.Message];
        }
    }
}
