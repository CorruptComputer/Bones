using Bones.Shared.Backend.Enums;

namespace Bones.Database.DbSets.AssetManagement;

/// <summary>
///     Model for the AssetManagement.AssetFieldListEntries table
/// </summary>
[Table("AssetFieldListEntries", Schema = "AssetManagement")]
[PrimaryKey(nameof(Id))]
public class AssetFieldListEntry
{
    /// <summary>
    ///     Internal ID for the AssetValueListEntry
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    public required string Value { get; set; }

    public required AssetField ParentField { get; set; }

    public required StringValueMatchingType MatchingType { get; set; }

    public bool Matches(string testStr)
    {
        return MatchingType switch
        {
            StringValueMatchingType.Exact => testStr.Equals(Value, StringComparison.InvariantCulture),
            StringValueMatchingType.CaseInvariant => testStr.Equals(Value, StringComparison.InvariantCultureIgnoreCase),
            _ => false
        };
    }
}