using System;

namespace Bones.WebUI.Components.Forms;

/// <summary>
///   A form for the bones project
/// </summary>
public partial class BonesForm : ComponentBase
{
    /// <summary>
    ///   The model to bind to
    /// </summary>
    [Parameter]
    public required object Model { get; set; }

    /// <summary>
    ///   The event to fire when the form is submitted
    /// </summary>
    [Parameter]
    public required EventCallback OnSubmit { get; set; }

    /// <summary>
    ///   The text to display on the submit button
    /// </summary>
    [Parameter]
    public required string SubmitText { get; set; }

    /// <summary>
    ///   The content of the form
    /// </summary>
    [Parameter]
    public required RenderFragment ChildContent { get; set; }

    /// <summary>
    ///   Whether to show the go back button
    /// </summary>
    [Parameter]
    public bool ShowGoBack { get; set; } = true;

    /// <summary>
    ///   The text to display on the go back button, defaults to "Cancel"
    /// </summary>
    [Parameter]
    public string GoBackText { get; set; } = "Cancel";
}
