namespace Bones.WebUI.Pages.SystemAdmin;

/// <summary>
///   Dashboard for sysadmins, something with: user count, organization count, project count, item count, and a few links to do sysadmin-y things
/// </summary>
public partial class SysAdminDashboardPage(BonesApiClient ApiClient) : ComponentBase
{
    /// <summary>
    ///   The number of users in the system, received from the API
    /// </summary>
    public int? UserCount { get; set; }

    /// <summary>
    ///   The number of organizations in the system, received from the API
    /// </summary>
    public int? OrganizationCount { get; set; }

    /// <summary>
    ///   The number of projects in the system, received from the API
    /// </summary>
    public int? ProjectCount { get; set; }

    /// <summary>
    ///   The number of items in the system, received from the API
    /// </summary>
    public int? ItemCount { get; set; }


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
        GetSystemAdminDashboardResponse dashboardResponse = await ApiClient.GetSystemAdminDashboardAsync();

        UserCount = dashboardResponse.UserCount;
        OrganizationCount = dashboardResponse.OrganizationCount;
        ProjectCount = dashboardResponse.ProjectCount;
        ItemCount = dashboardResponse.ItemCount;
    }
}