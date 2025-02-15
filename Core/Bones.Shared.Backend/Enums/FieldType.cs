using GeoJSON.Text.Feature;
using GeoJSON.Text.Geometry;

namespace Bones.Shared.Backend.Enums;

/// <summary>
///   The type of field
/// </summary>
public enum FieldType
{
    /// <summary>
    ///   Should just be a <see cref="string" />
    /// </summary>
    Text = 0,

    /// <summary>
    ///   Should just be a <see cref="int" />
    /// </summary>
    Integer = 1,

    /// <summary>
    ///   Should just be a <see cref="double" />
    /// </summary>
    Decimal = 2,

    /// <summary>
    ///   Should just be a <see cref="bool" />
    /// </summary>
    Boolean = 3,

    /// <summary>
    ///   Should just be a <see cref="DateTimeOffset"/>
    /// </summary>
    DateTime = 4,

    /// <summary>
    ///   The value will just be a <see cref="string" />, however it will be validated against a list of allowed values
    /// </summary>
    ValueList = 5,

    /// <summary>
    ///   Depending on the <see cref="LocationType" /> this can be a <see cref="string" />, a <see cref="Point" />, or a <see cref="FeatureCollection" />
    /// </summary>
    GeoLocation = 6,
}