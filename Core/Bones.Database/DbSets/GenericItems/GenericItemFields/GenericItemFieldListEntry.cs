using Bones.Shared.Backend.Enums;
using Bones.Shared.Extensions;

namespace Bones.Database.DbSets.GenericItems.GenericItemFields;

/// <summary>
///     Model for the GenericItems.GenericItemFieldListEntries table
/// </summary>
[Table("GenericItemFieldListEntries", Schema = "GenericItems")]
[PrimaryKey(nameof(Id))]
public class GenericItemFieldListEntry
{
    /// <summary>
    ///     Internal ID for the ItemValueListEntry
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    /// <summary>
    ///   The value for this Field List Entry
    /// </summary>
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public required string Value { get; set; }

    /// <summary>
    ///   The parent field this belongs to
    /// </summary>
    public required GenericItemField ParentField { get; set; }

    /// <summary>
    ///   The matching type this entry uses
    /// </summary>
    public required StringValueMatchingType MatchingType { get; set; }

    /// <summary>
    ///   Checks if the provided string matches the value for this entry
    /// </summary>
    /// <param name="testStr">The string to compare the Value to</param>
    /// <returns>[true] if it matches, [false] otherwise</returns>
    public bool Matches(string testStr)
    {
        return MatchingType switch
        {
            StringValueMatchingType.Exact => testStr.Equals(Value, StringComparison.InvariantCulture),
            StringValueMatchingType.CaseInvariant => testStr.Equals(Value, StringComparison.InvariantCultureIgnoreCase),
            StringValueMatchingType.Soundex => testStr.SoundexMatch(Value),
            _ => false
        };
    }
}