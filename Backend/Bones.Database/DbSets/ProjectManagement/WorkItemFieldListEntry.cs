using Bones.Shared.Backend.Enums;
using Bones.Shared.Extensions;

namespace Bones.Database.DbSets.ProjectManagement;

/// <summary>
///     Model for the ProjectManagement.WorkItemFieldListEntries table
/// </summary>
[Table("WorkItemFieldListEntries", Schema = "ProjectManagement")]
[PrimaryKey(nameof(Id))]
public class WorkItemFieldListEntry
{
    /// <summary>
    ///     Internal ID for the ItemValueListEntry
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    /// <summary>
    ///   The value this entry has
    /// </summary>
    public required string Value { get; set; }

    /// <summary>
    ///   The parent field this entry belongs to
    /// </summary>
    public required WorkItemField ParentField { get; set; }

    /// <summary>
    ///   The matching type used for the Value of this entry
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