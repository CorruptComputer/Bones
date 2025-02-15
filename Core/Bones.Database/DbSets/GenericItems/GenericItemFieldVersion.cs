using Bones.Database.DbConsts;
using Bones.Shared.Backend.Enums;

namespace Bones.Database.DbSets.GenericItems;

/// <summary>
///     Model for the GenericItems.GenericItemFields table
/// </summary>
[Table(TableNames.GenericItem.GenericItemFieldVersions, Schema = SchemaNames.GenericItem)]
[PrimaryKey(nameof(Id))]
public class GenericItemFieldVersion
{
    /// <summary>
    ///     Internal ID for the ItemFieldVersion
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    /// <summary>
    ///   The date and time this field version was created
    /// </summary>
    public DateTimeOffset CreateDateTime { get; init; } = DateTimeOffset.Now;

    /// <summary>
    ///   The ID of the layout this version belongs to
    /// </summary>
    [ForeignKey(nameof(GenericItemField))]
    public required Guid GenericItemFieldId { get; init; }

    /// <summary>
    ///   The version number for this
    /// </summary>
    public required long Version { get; init; }

    /// <summary>
    ///   The name of this ItemFieldVersion
    /// </summary>
    [MaxLength(512)]
    public required string Name { get; set; }

    /// <summary>
    ///   The FieldType for this field
    /// </summary>
    public required FieldType Type { get; set; }

    /// <summary>
    ///   Is this field required to have a value?
    /// </summary>
    public bool IsRequired { get; set; } = false;

    /// <summary>
    ///   If the Type of this field is either an Integer or Decimal, can it be negative?
    /// </summary>
    public bool? CanBeNegative { get; set; }

    /// <summary>
    ///   If the Type of this field is a ValueList, the possible values this can have
    /// </summary>
    public List<GenericItemFieldListEntry>? PossibleValues { get; set; }

    /// <summary>
    ///   If the FieldType is GeoLocation, the type of GeoLocation
    /// </summary>
    public GeoLocationType? GeoLocationType { get; set; }

    /// <summary>
    ///   If the GeoLocationType is Address, the required fields for the address
    /// </summary>
    public AddressFields? RequiredAddressFields { get; set; }

    /// <summary>
    ///   Disables creating of new layouts with this field,
    ///   and when all items using it are deleted it will be removed.
    /// </summary>
    public bool DeleteFlag { get; set; } = false;
}