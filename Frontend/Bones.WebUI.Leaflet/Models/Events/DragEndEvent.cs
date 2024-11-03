namespace Bones.WebUI.Leaflet.Models.Events;

/// <summary>
///   The event for when the map stops being dragged
/// </summary>
public class DragEndEvent : Event
{
    /// <summary>
    ///   The distance they dragged it
    /// </summary>
    public float Distance { get; init; }
}