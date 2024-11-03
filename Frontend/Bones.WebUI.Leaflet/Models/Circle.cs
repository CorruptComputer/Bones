namespace Bones.WebUI.Leaflet.Models;

/// <summary>
///   A circle, its round
/// </summary>
public class Circle : Path
{

    /// <summary>
    /// Center of the circle.
    /// </summary>
    public LatLon Position { get; set; } = new();

    /// <summary>
    /// Radius of the circle, in meters.
    /// </summary>
    public float Radius { get; set; }

}