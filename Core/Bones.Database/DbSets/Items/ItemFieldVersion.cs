using Bones.Database.DbConsts;
using Bones.Shared.Backend.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bones.Database.DbSets.Items;

/// <summary>
///   Model for the Item.ItemFields table
/// </summary>
[Table(TableNames.Item.ItemFieldVersions, Schema = SchemaNames.Item)]
[PrimaryKey(nameof(Id))]
public class ItemFieldVersion
{
    /// <summary>
    ///   Internal ID for the ItemFieldVersion
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    /// <summary>
    ///   The date and time this field version was created
    /// </summary>
    public DateTimeOffset CreateDateTime { get; init; } = DateTimeOffset.Now;

    /// <summary>
    ///   The ID of the ItemField this version belongs to
    /// </summary>
    public required Guid ItemFieldId { get; init; }

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

    #region Navigational Properties
    /// <summary>
    ///   Navigational property to the ItemField this version belongs to, null if not .Include()'d in the query
    /// </summary>
    public ItemField? ItemField { get; set; }

    /// <summary>
    ///   Navigational property to the possible values for this field version, empty if not .Include()'d in the query or not valid for this item type
    /// </summary>
    public List<ItemFieldListEntry> PossibleValues { get; set; } = [];
    #endregion

    internal static void BuildTable(EntityTypeBuilder<ItemFieldVersion> builder)
    {
        // Remove deleted items from being included in default queries
        builder.HasQueryFilter(x => !x.DeleteFlag);

        builder.HasOne(ifv => ifv.ItemField)
               .WithMany(itf => itf.Versions)
               .HasForeignKey(ifv => ifv.ItemFieldId);

        builder.HasMany(ifv => ifv.PossibleValues)
               .WithOne(pv => pv.ItemFieldVersion)
               .HasForeignKey(pv => pv.ItemFieldVersionId);
    }
}