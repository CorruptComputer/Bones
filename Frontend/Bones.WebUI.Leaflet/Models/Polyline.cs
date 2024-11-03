using System.Drawing;

namespace Bones.WebUI.Leaflet.Models;

/// <summary>
///   A poly line
/// </summary>
/// <typeparam name="TShape"></typeparam>
public class Polyline<TShape> : Path
{
    /// <summary>
    ///   The shape of the polyline
    /// </summary>
    public TShape? Shape { get; set; }

    /// <summary>
    /// How much to simplify the polyline on each zoom level. More means better performance and smoother look, and less means more accurate representation.
    /// </summary>
    public double SmoothFactory { get; set; } = 1.0;

    /// <summary>
    /// Disable polyline clipping.
    /// </summary>
    public bool NoClipEnabled { get; set; }

}

/// <summary>
///   A default type of polyline when no generic is defined for it
/// </summary>
public class Polyline : Polyline<PointF[][]>
{ }