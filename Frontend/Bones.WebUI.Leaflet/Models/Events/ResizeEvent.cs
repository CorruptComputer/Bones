using System.Drawing;

namespace Bones.WebUI.Leaflet.Models.Events;

/// <summary>
///   An event that something has been resized
/// </summary>
public class ResizeEvent : Event
{
    /// <summary>
    ///   The previous size
    /// </summary>
    public PointF? OldSize { get; init; }

    /// <summary>
    ///   The updated size
    /// </summary>
    public PointF? NewSize { get; init; }
}