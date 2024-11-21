using GeoJSON.Text.Feature;

namespace Bones.Shared.Backend.Enums;

/// <summary>
///   Type of location specified
/// </summary>
public enum LocationType
{
    /// <summary>
    ///   The value for this should be a <see cref="string" />
    /// </summary>
    StreetAddress = 0,

    /// <summary>
    ///   The value for this should be a <see cref="GeoJSON.Text.Geometry.Point" />
    /// </summary>
    Point = 1,

    /// <summary>
    ///   The value for this should be a <see cref="FeatureCollection" />
    /// </summary>
    CustomGeoJson = 2
}