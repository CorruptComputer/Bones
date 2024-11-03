namespace Bones.WebUI.Leaflet.Models.Events;

/// <summary>
///   An event that should display a tooltip
/// </summary>
public class TooltipEvent : Event
{
    /// <summary>
    ///   The tooltip that should be displayed
    /// </summary>
    public Tooltip? Tooltip { get; init; }
}