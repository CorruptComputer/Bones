using Bones.Shared.Backend.Enums;

namespace Bones.Database.DbSets.AssetManagement;

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

    [MaxLength(512)]
    public required string Name { get; set; }

    public bool IsRequired { get; set; } = false;

    public List<AssetFieldListEntry> PossibleValues { get; set; } = [];

    public required FieldType Type { get; set; }

}