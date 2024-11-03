using System.Drawing;

namespace Bones.WebUI.Leaflet.Models.Events;

/// <summary>
///   An event from the mouse
/// </summary>
public class MouseEvent : Event
{
    /// <summary>
    ///   The LatLon on the map where this took place
    /// </summary>
    public LatLon? LatLon { get; init; }

    /// <summary>
    ///   The PointF for the layer
    /// </summary>
    public PointF? LayerPoint { get; init; }

    /// <summary>
    ///   The PointF for the container
    /// </summary>
    public PointF? ContainerPoint { get; init; }

}