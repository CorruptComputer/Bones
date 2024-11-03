namespace Bones.WebUI.Leaflet.Enums;

/// <summary>
///   The current state of drawing
/// </summary>
public enum DrawState
{
    /// <summary>
    ///   Not drawing
    /// </summary>
    None,

    /// <summary>
    ///   Drawing a rectangle
    /// </summary>
    DrawingRectangle,

    /// <summary>
    ///   Drawing a circle
    /// </summary>
    DrawingCircle,

    /// <summary>
    ///   Drawing a freeform polygon
    /// </summary>
    DrawingPolygon
}