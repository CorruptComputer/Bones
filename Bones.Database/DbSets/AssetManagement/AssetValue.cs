using System.Drawing;
using System.Text.Json;
using Bones.Shared.Backend.Enums;
using Bones.Shared.Backend.Models;
using Bones.Shared.Exceptions;
using Bones.Shared.Extensions;
using GeoJSON.Text;
using GeoJSON.Text.Feature;

namespace Bones.Database.DbSets.AssetManagement;

/// <summary>
///     Model for the AssetManagement.AssetValues table
/// </summary>
[Table("AssetValues", Schema = "AssetManagement")]
[PrimaryKey(nameof(Id))]
public class AssetValue
{
    /// <summary>
    ///     Internal ID for the AssetValue
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    public required AssetField Field { get; set; }

    public LocationType? LocationType { get; set; }

    public string? Value { get; private set; }

    /// <summary>
    ///   Disables viewing this item value, and when safe to do so it will be removed.
    /// </summary>
    public bool DeleteFlag { get; set; } = false;

    public bool TrySetValue<T>(T? value)
    {
        if (value == null)
        {
            if (Field.IsRequired)
            {
                return false;
            }

            if (Field.Type == FieldType.GeoLocation && LocationType == null)
            {
                return false;
            }

            Value = null;
            return true;
        }

        switch (Field.Type)
        {
            case FieldType.String:
                Value = value.ToString();
                return true;
            case FieldType.Number:
                if (value.IsNumericType())
                {
                    Value = value.ToString();
                    return true;
                }

                return false;
            case FieldType.Boolean:
                if (value is bool)
                {
                    Value = value.ToString();
                    return true;
                }

                return false;
            case FieldType.DateTime:
                if (value is DateTime or DateTimeOffset)
                {
                    Value = value.ToString();
                    return true;
                }

                return false;
            case FieldType.ValueList:
                if (Field.PossibleValues?.Any(pv => pv.Matches(value.ToString() ?? string.Empty)) ?? false)
                {
                    Value = value.ToString();
                    return true;
                }

                return false;
            case FieldType.GeoLocation:
                switch (LocationType)
                {
                    case Shared.Backend.Enums.LocationType.StreetAddress:
                        Value = value.ToString();
                        return true;
                    case Shared.Backend.Enums.LocationType.Point:
                        try
                        {
                            Point? point = JsonSerializer.Deserialize<Point?>(value.ToString() ?? string.Empty);
                            if (point is not null)
                            {
                                Value = value.ToString();
                                return true;
                            }
                        }
                        catch
                        {
                            return false;
                        }

                        return false;
                    case Shared.Backend.Enums.LocationType.CustomGeoJson:
                        try
                        {
                            FeatureCollection? featureCollection = JsonSerializer.Deserialize<FeatureCollection?>(value.ToString() ?? string.Empty);
                            if (featureCollection is not null)
                            {
                                Value = value.ToString();
                                return true;
                            }
                        }
                        catch
                        {
                            return false;
                        }

                        return false;
                    default:
                        return false;
                }
            default:
                return false;
        }
    }

    public T? GetValue<T>()
    {
        if (Value == null)
        {
            return default;
        }

        switch (Field.Type)
        {
            case FieldType.String:
                if (typeof(T) != typeof(string))
                {
                    throw new BonesException("String value type must be string.");
                }

                return (T?)Convert.ChangeType(Value, typeof(T));
            case FieldType.Number:
                if (!typeof(T).IsNumericType())
                {
                    throw new BonesException("Number value type must be numeric.");
                }

                return (T?)Convert.ChangeType(Value, typeof(T));
            case FieldType.Boolean:
                if (typeof(T) != typeof(bool))
                {
                    throw new BonesException("Boolean value type must be bool.");
                }

                return (T?)Convert.ChangeType(Value, typeof(T));
            case FieldType.DateTime:
                if (typeof(T) != typeof(DateTime) || typeof(T) != typeof(DateTimeOffset))
                {
                    throw new BonesException("DateTime value type must be DateTime or DateTimeOffset.");
                }

                return (T?)Convert.ChangeType(Value, typeof(T));
            case FieldType.ValueList:
                if (typeof(T) != typeof(string))
                {
                    throw new BonesException("ValueList value type must be string.");
                }

                return (T?)Convert.ChangeType(Value, typeof(T));
            case FieldType.GeoLocation:
                switch (LocationType)
                {
                    case Shared.Backend.Enums.LocationType.StreetAddress:
                        if (typeof(T) != typeof(string))
                        {
                            throw new BonesException("GeoLocation StreetAddress value type must be string.");
                        }

                        return (T?)Convert.ChangeType(Value, typeof(T));
                    case Shared.Backend.Enums.LocationType.Point:
                        if (typeof(T) != typeof(Point))
                        {
                            throw new BonesException("GeoLocation Point value type must be Point.");
                        }

                        try
                        {
                            Point? point = JsonSerializer.Deserialize<Point?>(Value);
                            if (point is not null)
                            {
                                return (T?)Convert.ChangeType(point, typeof(T));
                            }
                        }
                        catch
                        {
                            return default;
                        }

                        return default;
                    case Shared.Backend.Enums.LocationType.CustomGeoJson:
                        try
                        {
                            FeatureCollection? featureCollection = JsonSerializer.Deserialize<FeatureCollection?>(Value);
                            if (featureCollection is not null)
                            {
                                return (T?)Convert.ChangeType(featureCollection, typeof(T));
                            }
                        }
                        catch
                        {
                            return default;
                        }

                        return default;
                    default:
                        return default;
                }
            default:
                return default;
        }
    }
}