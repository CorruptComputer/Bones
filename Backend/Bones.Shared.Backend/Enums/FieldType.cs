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
    String = 0,

    /// <summary>
    ///   Any numeric type should work for this:
    ///   <see cref="int" />,
    ///   <see cref="double" />,
    ///   <see cref="float" />,
    ///   etc.
    /// </summary>
    Number = 1,

    /// <summary>
    ///   Should just be a <see cref="bool" />
    /// </summary>
    Boolean = 2,

    /// <summary>
    ///   Should be either a <see cref="DateTime" />, or <see cref="DateTimeOffset"/>
    /// </summary>
    DateTime = 3,

    /// <summary>
    ///   The value will just be a <see cref="string" />, however it will be validated against a list of allowed values
    /// </summary>
    ValueList = 4,

    /// <summary>
    ///   Depending on the <see cref="LocationType" /> this can be a <see cref="string" />, a <see cref="Point" />, or a <see cref="FeatureCollection" />
    /// </summary>
    GeoLocation = 5,
}