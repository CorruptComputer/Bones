namespace Bones.WebUI.Components.Item.Assignment;

/// <summary>
///   Page for viewing an items assignees
/// </summary>
public partial class ItemAssignee : ComponentBase
{
    /// <summary>
    ///   The assignment slot model for this assignee
    /// </summary>
    [Parameter]
    public required AssignmentSlotModel AssignmentSlot { get; set; }

    /// <summary>
    ///   The assignee for this, null if not assigned
    /// </summary>
    [Parameter]
    public required AssigneeModel? Assignee { get; set; }

    /// <summary>
    ///   Fires when the component is loaded
    /// </summary>
    /// <returns></returns>
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
    }
}
