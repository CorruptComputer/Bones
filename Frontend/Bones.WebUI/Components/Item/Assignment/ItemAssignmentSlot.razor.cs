using System.Net;
using ReQuesty.Runtime.Abstractions;

namespace Bones.WebUI.Components.Item.Assignment;

/// <summary>
///   Page for viewing an items assignees
/// </summary>
public partial class ItemAssignmentSlot : ComponentBase
{
    /// <summary>
    ///   The assignment slot model
    /// </summary>
    [Parameter]
    public required AssignmentSlotModel AssignmentSlot { get; set; }

    /// <summary>
    ///   The assignees for this
    /// </summary>
    [Parameter]
    public required List<AssigneeModel?> Assignees { get; set; }

    /// <summary>
    ///   Fires when the component is loaded
    /// </summary>
    /// <returns></returns>
    protected override async Task OnInitializedAsync()
    {
        if (AssignmentSlot.SelectionSlotType is SelectionType.Single)
        {
            if (Assignees.Count == 0)
            {
                Assignees.Add(null);
            }
        }
        else if (AssignmentSlot.SelectionSlotType is SelectionType.Multiple)
        {
            // No max yet, just add a null at the end to give the option to add more
            Assignees.Add(null);
        }

        await base.OnInitializedAsync();
    }
}
