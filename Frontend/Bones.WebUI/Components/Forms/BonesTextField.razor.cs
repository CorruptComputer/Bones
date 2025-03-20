using MudBlazor;

namespace Bones.WebUI.Components.Forms;

/// <summary>
///   A text field component for the bones project
/// </summary>
public partial class BonesTextField : ComponentBase
{
    /// <summary>
    ///   The label of the text field
    /// </summary>
    [Parameter]
    public required string Label { get; set; }

    /// <summary>
    ///   The type of the text field
    /// </summary>
    [Parameter]
    public required InputType InputType { get; set; }

    /// <summary>
    ///   The helper text of the text field
    /// </summary>
    [Parameter]
    public string? HelperText { get; set; }

    /// <summary>
    ///   The value of the text field
    /// </summary>
    [Parameter]
    public required string Value { get; set; }

    /// <summary>
    ///   The event that is triggered when the value of the text field changes
    /// </summary>
    [Parameter]
    public required EventCallback<string> ValueChanged { get; set; }
}
