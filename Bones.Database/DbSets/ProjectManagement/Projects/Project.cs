using Bones.Database.DbSets.ProjectManagement.Initiatives;

namespace Bones.Database.DbSets.ProjectManagement.Projects;

/// <summary>
///     Model for the ProjectManagement.Projects table
/// </summary>
[Table("Projects", Schema = "ProjectManagement")]
[PrimaryKey(nameof(Id))]
public class Project
{
    /// <summary>
    ///     Internal ID for the Tag
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [MaxLength(512)]
    public required string Name { get; set; }

    public List<Initiative> Initiatives { get; set; } = [];

    /// <summary>
    ///   Disables access to this Project and schedules deletes for everything within,
    ///   when all items using it are deleted it will be removed.
    /// </summary>
    public bool DeleteFlag { get; set; } = false;
}