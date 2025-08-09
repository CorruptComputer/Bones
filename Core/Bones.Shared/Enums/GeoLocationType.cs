namespace Bones.Shared.Enums;

/// <summary>
///   The type of GeoLocation information
/// </summary>
public enum GeoLocationType
{
    /// <summary>
    ///   An auto-updating OSM object, pulls the latest geometry from OSM
    /// </summary>
    OsmObject = 0,

    /// <summary>
    ///   A specific latitude and longitude
    /// </summary>
    LatLon = 1,

    /// <summary>
    ///   An address, may or may not have any single address field
    /// </summary>
    Address = 2,

    /// <summary>
    ///   Some bit of static geometry
    /// </summary>
    Geometry = 3
}
