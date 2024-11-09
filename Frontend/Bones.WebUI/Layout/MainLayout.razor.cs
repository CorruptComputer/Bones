namespace Bones.WebUI.Layout;

/// <summary>
/// 
/// </summary>
public partial class MainLayout
{
    private bool _open = false;

    private record ProjectDropDownModel
    {
        public required string OrganizationName { get; init; }
        public required string ProjectName { get; init; }
        
        public required Guid? ProjectId { get; init; }

        public override string ToString()
        {
            if (ProjectId == null || ProjectId == Guid.Empty)
            {
                return OrganizationName;
            }
            
            return $"{OrganizationName} - {ProjectName}";
        }
    }

    private List<ProjectDropDownModel> Projects { get; set; } = [];

    /// <summary>
    ///   
    /// </summary>
    protected override void OnInitialized()
    {
        Projects.Add(new()
        {
            OrganizationName = "+ Create a new project",
            ProjectName = string.Empty,
            ProjectId = null
        });
        
        // TODO: Get users list of projects from the API and add them
        
        Projects.Add(new()
        {
            OrganizationName = "+ Create a new project",
            ProjectName = string.Empty,
            ProjectId = Guid.Empty
        });
        base.OnInitialized();
    }

    private void ToggleDrawer()
    {
        _open = !_open;
    }
    
    private void OnGoToProjectChanged(IEnumerable<Guid?>? selectedProject)
    {
        Guid? selected = selectedProject?.FirstOrDefault();
        if (selected.HasValue)
        {
            if (selected.Value == Guid.Empty)
            {
                // TODO:
                // NavManager.NavigateTo(FrontEndUrls.Projects.CREATE);
            }
            else
            {
                // TODO: 
                // NavManager.NavigateTo(FrontEndUrls.Projects.DASHBOARD.Replace("{{ProjectId}}", selected.Value.ToString());
            }
        }
    }
}