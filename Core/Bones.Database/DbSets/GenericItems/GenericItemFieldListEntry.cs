using Bones.Database.DbConsts;
using Bones.Shared.Enums;
using Bones.Shared.Extensions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bones.Database.DbSets.GenericItems;

/// <summary>
///     Model for the GenericItems.GenericItemFieldListEntries table
/// </summary>
[Table(TableNames.GenericItem.GenericItemFieldListEntries, Schema = SchemaNames.GenericItem)]
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
    ///   The parent field version this belongs to
    /// </summary>
    public required Guid GenericItemFieldVersionId { get; set; }

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

    /// <summary>
    ///   Disables creating of new layouts with this entry value,
    ///   and when all items using it are deleted it will be removed.
    /// </summary>
    public bool DeleteFlag { get; set; } = false;

    internal static void BuildTable(EntityTypeBuilder<GenericItemFieldListEntry> builder)
    {
        // Remove deleted items from being included in default queries
        builder.HasQueryFilter(x => !x.DeleteFlag);
    }
}