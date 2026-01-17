using Bones.Database.DbConsts;
using Bones.Shared.Backend.Enums;
using Bones.Shared.Extensions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bones.Database.DbSets.Items;

/// <summary>
///   Model for the Item.ItemFieldListEntries table
/// </summary>
[Table(TableNames.Item.ItemFieldListEntries, Schema = SchemaNames.Item)]
[PrimaryKey(nameof(Id))]
public class ItemFieldListEntry
{
    /// <summary>
    ///   Internal ID for the ItemValueListEntry
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
    public required Guid ItemFieldVersionId { get; set; }

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

    #region Navigational Properties
    /// <summary>
    ///   Navigational property to the ItemFieldVersion this entry belongs to, null if not .Include()'d in the query
    /// </summary>
    public ItemFieldVersion? ItemFieldVersion { get; set; }
    #endregion

    internal static void BuildTable(EntityTypeBuilder<ItemFieldListEntry> builder)
    {
        // Remove deleted items from being included in default queries
        builder.HasQueryFilter(x => !x.DeleteFlag);

        builder.HasOne(ifle => ifle.ItemFieldVersion)
               .WithMany(ifv => ifv.PossibleValues)
               .HasForeignKey(ifle => ifle.ItemFieldVersionId);
    }
}