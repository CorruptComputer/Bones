using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.OrganizationManagement;
using Bones.Shared.Backend.Enums;

namespace Bones.Database.DbSets.AssetManagement.AssetFields;

/// <summary>
///     Model for the AssetManagement.AssetFields table
/// </summary>
[Table("AssetFields", Schema = "AssetManagement")]
[PrimaryKey(nameof(Id))]
public class AssetField
{
    /// <summary>
    ///     Internal ID for the AssetField
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    /// <summary>
    ///   The name of this asset field
    /// </summary>
    [MaxLength(512)]
    public required string Name { get; set; }

    /// <summary>
    ///   The type of owner this asset field has
    /// </summary>
    public OwnershipType OwnerType { get; set; }

    /// <summary>
    ///   If the OwnerType is a User, the user that owns this asset field
    /// </summary>
    public BonesUser? OwningUser { get; set; }

    /// <summary>
    ///   If the OwnerType is an Organization, the organization that owns this asset field
    /// </summary>
    public BonesOrganization? OwningOrganization { get; set; }

    /// <summary>
    ///   Is this field required to have a value?
    /// </summary>
    public bool IsRequired { get; set; } = false;

    /// <summary>
    ///   If the Type of this field is a ValueList, the possible values this can have
    /// </summary>
    public List<AssetFieldListEntry>? PossibleValues { get; set; }

    /// <summary>
    ///   The FieldType for this field
    /// </summary>
    public required FieldType Type { get; set; }

}