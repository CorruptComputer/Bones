using Bones.Shared.Backend.Enums;

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

    public required string Value { get; set; }

    public required WorkItemField ParentField { get; set; }

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