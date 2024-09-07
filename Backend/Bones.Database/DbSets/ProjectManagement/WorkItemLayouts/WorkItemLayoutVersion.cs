using Bones.Database.DbSets.ProjectManagement.WorkItemFields;

namespace Bones.Database.DbSets.ProjectManagement.WorkItemLayouts;

/// <summary>
///     Model for the ProjectManagement.WorkItemLayoutVersions table
/// </summary>
[Table("WorkItemLayoutVersions", Schema = "ProjectManagement")]
[PrimaryKey(nameof(Id))]
public class WorkItemLayoutVersion
{
    /// <summary>
    ///     Internal ID for the ItemLayoutVersion
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; init; }

    /// <summary>
    ///   The layout this version belongs to
    /// </summary>
    public required WorkItemLayout WorkItemLayout { get; init; }

    /// <summary>
    ///   The version number for this layout
    /// </summary>
    public required long Version { get; init; }

    /// <summary>
    ///   The fields this layout version has
    /// </summary>
    public List<WorkItemField> Fields { get; init; } = [];

    /// <summary>
    ///   Disables creating of new items using this layout version,
    ///   and when all items using it are deleted it will be removed.
    /// </summary>
    public bool DeleteFlag { get; set; } = false;
}