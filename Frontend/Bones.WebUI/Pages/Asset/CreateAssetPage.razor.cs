using System.Diagnostics.CodeAnalysis;
using Bones.WebUI.Models;

namespace Bones.WebUI.Pages.Asset;

/// <summary>
///   Page for creating a new asset
/// </summary>
public partial class CreateAssetPage(BonesApiClient apiClient) : ComponentBase
{
    /// <summary>
    ///   The ID of the project to load in this dashboard
    /// </summary>
    [Parameter]
    [SupplyParameterFromQuery(Name = "layout-id")]
    public Guid? AssetLayoutId { get; set; }

    /// <summary>
    ///   Is the asset layout ID provided in the query string?
    /// </summary>
    [MemberNotNullWhen(true, nameof(AssetLayoutId))]
    protected bool AssetLayoutIdProvidedInQueryString { get; set; }

    /// <summary>
    ///   The projects dropdown values
    /// </summary>
    protected List<DropDownModel> Projects { get; set; } = [
        new()
        {
            DisplayStr = "(loading)",
            Id = null
        }];

    /// <summary>
    ///   The project selected by the user
    /// </summary>
    protected Guid? SelectedProject { get; set; }

    /// <summary>
    ///   The asset layout dropdown values
    /// </summary>
    protected List<DropDownModel> AssetLayouts { get; set; } = [
        new()
        {
            DisplayStr = "(loading)",
            Id = null
        }];

    /// <summary>
    ///   The asset layout selected by the user
    /// </summary>
    protected Guid? SelectedAssetLayout { get; set; }


    /// <summary>
    ///   Did the request to the API result in an error?
    /// </summary>
    protected bool ApiError { get; set; } = false;

    /// <summary>
    ///   Is the form valid?
    /// </summary>
    protected bool FormValid { get; set; }

    /// <summary>
    ///   The issues with the users inputs
    /// </summary>
    protected string[] ValidationErrors { get; set; } = [];

    /// <summary>
    ///   The name of the work item queue, received from the API
    /// </summary>
    protected string? AssetLayoutName { get; set; }

    /// <summary>
    ///   Fires when the page is loaded
    /// </summary>
    /// <returns></returns>
    protected override async Task OnInitializedAsync()
    {
        AssetLayoutIdProvidedInQueryString = AssetLayoutId.HasValue;
        await FetchFromAPI();

        await base.OnInitializedAsync();
    }

    /// <summary>
    ///   Fires if the same page but with a different parameter is loaded
    /// </summary>
    /// <returns></returns>
    protected override async Task OnParametersSetAsync()
    {
        AssetLayoutIdProvidedInQueryString = AssetLayoutId.HasValue;
        await FetchFromAPI();

        await base.OnParametersSetAsync();
    }

    private async Task FetchFromAPI()
    {
        if (AssetLayoutIdProvidedInQueryString)
        {
            GetItemLayoutVersionResponse layout = await apiClient.GenericItem.Layouts[AssetLayoutId.Value].Latest.GetAsync()
                ?? throw new InvalidOperationException("Failed to get layout from API");
            SelectedProject = layout.ProjectId;
        }
        else
        {
            await GetProjects();
        }
    }

    private async Task GetProjects()
    {
        List<GetProjectQuickSelectResponse>? resp = await apiClient.Project.Projects.QuickSelect.GetAsync();

        if (resp is null)
        {
            ApiError = true;
            return;
        }

        Projects = [.. resp.Select(x => new DropDownModel
        {
            DisplayStr = x.ProjectName,
            Id = x.ProjectId
        })];
    }

    private async Task GetAssetLayouts(Guid selectedProject)
    {
        List<GetProjectLayoutsResponse>? resp = await apiClient.Project[selectedProject].Layouts.GetAsync(req => req.QueryParameters.LayoutUse = ItemLayoutUse.Assets);

        if (resp is null)
        {
            ApiError = true;
            return;
        }

        AssetLayouts = [.. resp.Select(x => new DropDownModel
        {
            DisplayStr = $"{x.FriendlyIdPrefix} - {x.LayoutName}",
            Id = x.LayoutId
        })];
    }

    /// <summary>
    ///   Event for when the selected project is changed
    /// </summary>
    /// <param name="selectedProject"></param>
    /// <returns></returns>
    protected async Task OnSelectedProjectChanged(Guid? selectedProject)
    {
        if (selectedProject.HasValue)
        {
            SelectedProject = selectedProject.Value;
            await GetAssetLayouts(SelectedProject.Value);
        }
    }

    /// <summary>
    ///   Event for when the selected work item layout is changed
    /// </summary>
    /// <param name="selectedLayout"></param>
    /// <returns></returns>
    protected async Task OnSelectedLayoutChanged(Guid? selectedLayout)
    {
        if (selectedLayout.HasValue)
        {
            SelectedAssetLayout = selectedLayout.Value;
            await Task.CompletedTask;
        }
    }
}
