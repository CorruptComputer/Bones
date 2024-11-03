namespace Bones.WebUI.Leaflet.Models;

/// <summary>
///   A layer that holds GeoJson data
/// </summary>
public class GeoJsonDataLayer : InteractiveLayer
{
    /// <summary>
    ///   The GeoJson data
    /// </summary>
    public string GeoJsonData { get; set; } = string.Empty;
}