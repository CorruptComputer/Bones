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

    /// <summary>
    ///   The layout version this work item version uses
    /// </summary>
    public required WorkItemLayoutVersion WorkItemLayout { get; set; }

    /// <summary>
    ///   The version number for this item version
    /// </summary>
    public required long Version { get; set; }

    /// <summary>
    ///   The parent work item that this version belongs to
    /// </summary>
    public required WorkItem WorkItem { get; set; }

    /// <summary>
    ///   The field values that this version has
    /// </summary>
    public required List<WorkItemValue> Values { get; set; }

    /// <summary>
    ///   Disables viewing this item version, and when safe to do so it will be removed.
    /// </summary>
    public bool DeleteFlag { get; set; } = false;
}