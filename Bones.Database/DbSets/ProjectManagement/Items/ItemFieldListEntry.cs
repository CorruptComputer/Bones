namespace Bones.Database.DbSets.ProjectManagement.Items;

/// <summary>
///     Model for the ProjectManagement.ItemFieldListEntries table
/// </summary>
[Table("ItemFieldListEntries", Schema = "ProjectManagement")]
[PrimaryKey(nameof(Id))]
public class ItemFieldListEntry
{
    /// <summary>
    ///     Internal ID for the ItemValueListEntry
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    public required string Value { get; set; }

    public required ItemField ParentField { get; set; }

    public required MatchType MatchingType { get; set; }

    public enum MatchType
    {
        Exact,
        CaseInvariant
    }

    public bool Matches(string testStr)
    {
        return MatchingType switch
        {
            MatchType.Exact => testStr.Equals(Value, StringComparison.InvariantCulture),
            MatchType.CaseInvariant => testStr.Equals(Value, StringComparison.InvariantCultureIgnoreCase),
            _ => false
        };
    }
}