using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.OrganizationManagement;
using Bones.Shared.Backend.Enums;

namespace Bones.Database.DbSets.ProjectManagement;

/// <summary>
///     Model for the ProjectManagement.WorkItemLayouts table
/// </summary>
[Table("WorkItemLayouts", Schema = "ProjectManagement")]
[PrimaryKey(nameof(Id))]
public class WorkItemLayout
{
    /// <summary>
    ///     Internal ID for the ItemLayout Version
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; init; }

    /// <summary>
    ///   The name for this layout
    /// </summary>
    [MaxLength(512)]
    public required string Name { get; set; }
    
    /// <summary>
    ///   Parent project for this layout
    /// </summary>
    public required Project Project { get; set; }

    /// <summary>
    ///   The versions for this layout
    /// </summary>
    public List<WorkItemLayoutVersion> Versions { get; set; } = [];

    /// <summary>
    ///   Disables creating of new items using this layout,
    ///   and when all items using it are deleted it will be removed.
    /// </summary>
    public bool DeleteFlag { get; set; } = false;
}