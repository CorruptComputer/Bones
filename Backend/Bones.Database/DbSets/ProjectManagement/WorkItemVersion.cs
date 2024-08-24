namespace Bones.Database.DbSets.ProjectManagement;

/// <summary>
///     Model for the ProjectManagement.WorkItemVersions table
/// </summary>
[Table("WorkItemVersions", Schema = "ProjectManagement")]
[PrimaryKey(nameof(Id))]
public class WorkItemVersion
{
    /// <summary>
    ///     Internal ID for the ItemVersion
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    public required WorkItemLayoutVersion WorkItemLayout { get; set; }

    public required long Version { get; set; }

    public required WorkItem WorkItem { get; set; }

    public required List<WorkItemValue> Values { get; set; }

    /// <summary>
    ///   Disables viewing this item version, and when safe to do so it will be removed.
    /// </summary>
    public bool DeleteFlag { get; set; } = false;
}