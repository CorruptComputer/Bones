using System.Globalization;
using Bones.Database.DbConsts;
using Bones.Database.DbSets.MappingManagement;
using Bones.Shared.Backend.Enums;
using Bones.Shared.Exceptions;
using Bones.Shared.Extensions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bones.Database.DbSets.Items;

/// <summary>
///   Model for the Item.ItemValues table
/// </summary>
[Table(TableNames.Item.ItemValues, Schema = SchemaNames.Item)]
[PrimaryKey(nameof(Id))]
public class ItemValue
{
    /// <summary>
    ///   Internal ID for the ItemValue
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    /// <summary>
    ///   The date and time this item value was created
    /// </summary>
    public DateTimeOffset CreateDateTime { get; init; } = DateTimeOffset.Now;

    /// <summary>
    ///   The ID of the ItemVersion this value belongs to
    /// </summary>
    public required Guid ItemVersionId { get; init; }

    /// <summary>
    ///   The ID of the ItemFieldVersion this value belongs to
    /// </summary>
    public required Guid ItemFieldVersionId { get; init; }

    /// <summary>
    ///   If the field this value belongs to is a Location, the location info will be saved here
    /// </summary>
    public GeoLocation? Location { get; private set; }

    /// <summary>
    ///   The value this field holds
    /// </summary>
    public string? Value { get; private set; }

    /// <summary>
    ///   Disables viewing this item value, and when safe to do so it will be removed.
    /// </summary>
    public bool DeleteFlag { get; set; } = false;

    #region Navigational Properties
    /// <summary>
    ///   Navigational property to the ItemVersion this value belongs to, null if not .Include()'d in the query
    /// </summary>
    public ItemVersion? ItemVersion { get; set; }

    /// <summary>
    ///   Navigational property to the ItemFieldVersion this value belongs to, null if not .Include()'d in the query
    /// </summary>
    public ItemFieldVersion? ItemFieldVersion { get; set; }
    #endregion

    internal static void BuildTable(EntityTypeBuilder<ItemValue> builder)
    {
        // Remove deleted items from being included in default queries
        builder.HasQueryFilter(ival => !ival.DeleteFlag);

        builder.HasOne(ival => ival.ItemVersion)
            .WithMany(iver => iver.ItemValues)
            .HasForeignKey(ival => ival.ItemVersionId);

        builder.HasOne(ival => ival.ItemFieldVersion)
            .WithMany()
            .HasForeignKey(ival => ival.ItemFieldVersionId);
    }

    /// <summary>
    ///   Tries to set the value to the specified type, checking the Fields Type to ensure its valid.
    ///   Cannot be a null value, if the value is supposed to be null then just don't call this. It defaults to that.
    /// </summary>
    /// <param name="valueToSet">The value</param>
    /// <typeparam name="T">The type to use for the value</typeparam>
    /// <returns>A flag for if this was successful; true for success, and false for failure.</returns>
    public bool TrySetValue<T>(T valueToSet)
        where T : notnull
    {
        bool success = ItemFieldVersion?.Type switch
        {
            FieldType.TextField or FieldType.TextBox => ValidateAndSetTextValue(valueToSet),
            FieldType.Integer => ValidateAndSetIntegerValue(valueToSet),
            FieldType.Decimal => ValidateAndSetDecimalValue(valueToSet),
            FieldType.Boolean => ValidateAndSetBooleanValue(valueToSet),
            FieldType.DateTime => ValidateAndSetDateTimeValue(valueToSet),
            FieldType.ValueList => ValidateAndSetValueListValue(valueToSet),
            FieldType.GeoLocation => ValidateAndSetGeoLocationValue(valueToSet),
            _ => false
        };

        return success;
    }

    /// <summary>
    ///   Tries to get the type from the Value, checking that the type is valid for this value type
    /// </summary>
    /// <typeparam name="T">Type to return</typeparam>
    /// <returns>Hopefully what you asked for</returns>
    /// <exception cref="BonesException">Throw up, we can't digest this the type isn't valid</exception>
    public T? GetValue<T>()
    {
        if (ItemFieldVersion?.Type != FieldType.GeoLocation && Value == null)
        {
            return default;
        }
        else if (ItemFieldVersion?.Type == FieldType.GeoLocation && Location == null)
        {
            return default;
        }

        return ItemFieldVersion?.Type switch
        {
            FieldType.TextField or FieldType.TextBox => GetStringValue<T>(),
            FieldType.ValueList => GetStringValue<T>(),
            FieldType.Integer => GetIntegerValue<T>(),
            FieldType.Decimal => GetDecimalValue<T>(),
            FieldType.Boolean => GetBooleanValue<T>(),
            FieldType.DateTime => GetDateTimeValue<T>(),
            FieldType.GeoLocation => GetGeoLocationValue<T>(),
            _ => throw new BonesException($"Invalid ItemFieldVersion.Type '{ItemFieldVersion?.Type}' specified.")
        };
    }

    #region Set Value Helpers
    private bool ValidateAndSetTextValue<T>(T valueToSet)
        where T : notnull
    {
        if (valueToSet is string str)
        {
            Value = str;
            return true;
        }

        return false;
    }

    private bool ValidateAndSetIntegerValue<T>(T valueToSet)
        where T : notnull
    {
        if ((ItemFieldVersion?.CanBeNegative == true && valueToSet.IsSignedIntegerType())
            || (ItemFieldVersion?.CanBeNegative == false && valueToSet.IsUnsignedIntegerType()))
        {
            Value = valueToSet.ToString();
            return true;
        }

        return false;
    }

    private bool ValidateAndSetDecimalValue<T>(T valueToSet)
        where T : notnull
    {
        if (ItemFieldVersion?.CanBeNegative is not null && valueToSet.IsDecimalType())
        {
            if (ItemFieldVersion.CanBeNegative == false && Convert.ToDouble(valueToSet) < 0)
            {
                return false;
            }

            Value = valueToSet.ToString();
            return true;
        }

        return false;
    }

    private bool ValidateAndSetBooleanValue<T>(T valueToSet)
        where T : notnull
    {
        if (valueToSet is bool b)
        {
            Value = b.ToString();
            return true;
        }

        return false;
    }

    private bool ValidateAndSetDateTimeValue<T>(T valueToSet)
        where T : notnull
    {
        if (valueToSet is DateTimeOffset dto)
        {
            Value = dto.ToString(CultureInfo.InvariantCulture);
            return true;
        }

        return false;
    }

    private bool ValidateAndSetValueListValue<T>(T valueToSet)
        where T : notnull
    {
        if (valueToSet is string str
            && ItemFieldVersion?.PossibleValues is not null
            && ItemFieldVersion.PossibleValues.Any(pv => pv.Matches(str)))
        {
            Value = str;
            return true;
        }

        return false;
    }

    private bool ValidateAndSetGeoLocationValue<T>(T valueToSet)
        where T : notnull
    {
        if (valueToSet is GeoLocation geoLocation)
        {
            Value = null; // Just to be explicit here, these will NOT be stored in the Value column.
            Location = geoLocation;
            return true;
        }

        return false;
    }
    #endregion

    #region Get Value Helpers
    private T? GetStringValue<T>()
    {
        // Nothings set
        if (Value == null)
        {
            return default;
        }

        // This should only convert to a string
        if (typeof(T) != typeof(string))
        {
            throw new BonesException($"Cannot convert {ItemFieldVersion?.Type} to {typeof(T).Name}, try a string instead.");
        }

        return (T?)Convert.ChangeType(Value, typeof(T));
    }

    private T? GetIntegerValue<T>()
    {
        if (ItemFieldVersion?.CanBeNegative == true && typeof(T) == typeof(long))
        {
            return (T?)Convert.ChangeType(Value, typeof(long));
        }
        else if (ItemFieldVersion?.CanBeNegative == false && typeof(T) == typeof(ulong))
        {
            return (T?)Convert.ChangeType(Value, typeof(ulong));
        }

        throw new BonesException($"Cannot convert {ItemFieldVersion?.Type} to {typeof(T).Name}, try a long or ulong instead.");
    }

    private T? GetDecimalValue<T>()
    {
        if (typeof(T) == typeof(double))
        {
            return (T?)Convert.ChangeType(Value, typeof(double));
        }

        throw new BonesException($"Cannot convert {ItemFieldVersion?.Type} to {typeof(T).Name}, try a double instead.");
    }

    private T? GetBooleanValue<T>()
    {
        if (typeof(T) == typeof(bool))
        {
            return (T?)Convert.ChangeType(Value, typeof(bool));
        }

        throw new BonesException($"Cannot convert {ItemFieldVersion?.Type} to {typeof(T).Name}, try a bool instead.");
    }

    private T? GetDateTimeValue<T>()
    {
        if (typeof(T) == typeof(DateTimeOffset))
        {
            return (T?)Convert.ChangeType(Value, typeof(DateTimeOffset), CultureInfo.InvariantCulture);
        }

        throw new BonesException($"Cannot convert {ItemFieldVersion?.Type} to {typeof(T).Name}, try a DateTimeOffset instead.");
    }

    private T? GetGeoLocationValue<T>()
    {
        if (typeof(T) == typeof(GeoLocation))
        {
            return (T?)Convert.ChangeType(Location, typeof(GeoLocation));
        }

        throw new BonesException($"Cannot convert {ItemFieldVersion?.Type} to {typeof(T).Name}, try a GeoLocation instead.");
    }
    #endregion
}