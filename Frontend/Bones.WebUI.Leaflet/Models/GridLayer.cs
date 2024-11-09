using System.Drawing;

namespace Bones.WebUI.Leaflet.Models;

/// <summary>
///   A layer of grids
/// </summary>
public abstract class GridLayer : Layer
{

    /// <summary>
    /// Width and height of tiles in the grid.
    /// </summary>
    public Size TileSize { get; set; } = new(256, 256);

    /// <summary>
    ///   The opacity for this layer
    /// </summary>
    public double Opacity { get; set; } = 1.0;

    /// <summary>
    /// By default, a smooth zoom animation will update grid layers every integer zoom level. Setting this option to false will update the grid layer only when the smooth animation ends.
    /// </summary>
    public bool UpdateWhenZooming { get; set; } = true;

    /// <summary>
    /// Tiles will not update more than once every updateInterval milliseconds when panning.
    /// </summary>
    public int UpdateInterval { get; set; } = 200;

    /// <summary>
    ///   The altitude for this layer
    /// </summary>
    public int ZIndex { get; set; } = 1;

    /// <summary>
    /// If set, tiles will only be loaded inside the set.
    /// </summary>
    public ValueTuple<LatLon, LatLon>? Bounds { get; set; }
}