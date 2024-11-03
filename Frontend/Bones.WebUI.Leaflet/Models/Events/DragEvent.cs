namespace Bones.WebUI.Leaflet.Models.Events;

/// <summary>
///   The event for when the map is being dragged
/// </summary>
public class DragEvent : Event
{
    /// <summary>
    ///   The LatLon it is at now
    /// </summary>
    public LatLon? LatLon { get; init; }

    /// <summary>
    ///   The LatLon it was at before
    /// </summary>
    public LatLon? OldLatLon { get; init; }
}