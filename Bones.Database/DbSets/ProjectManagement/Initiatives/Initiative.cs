using Bones.Database.DbSets.ProjectManagement.Projects;
using Bones.Database.DbSets.ProjectManagement.Queues;

namespace Bones.Database.DbSets.ProjectManagement.Initiatives;

/// <summary>
///     Model for the ProjectManagement.Initiatives table
/// </summary>
[Table("Initiatives", Schema = "ProjectManagement")]
[PrimaryKey(nameof(Id))]
public class Initiative
{
    /// <summary>
    ///     Internal ID for the Tag
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    public required Project Project { get; set; }

    [MaxLength(512)]
    public required string Name { get; set; }

    public List<Queue> Queues { get; set; } = [];
}