using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.OrganizationManagement;
using Bones.Shared.Backend.Enums;

namespace Bones.Database.DbSets.ProjectManagement;

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

    /// <summary>
    ///   The name of the project
    /// </summary>
    [MaxLength(512)]
    public required string Name { get; set; }

    /// <summary>
    ///   The child initiatives this project has
    /// </summary>
    public List<Initiative> Initiatives { get; set; } = [];

    /// <summary>
    ///   The type of owner for this project
    /// </summary>
    public OwnershipType OwnerType { get; set; }

    /// <summary>
    ///   If the OwnerType is User, the user that owns this project
    /// </summary>
    public BonesUser? OwningUser { get; set; }

    /// <summary>
    ///   If the OwnerType is Organization, the organization that owns this project
    /// </summary>
    public BonesOrganization? OwningOrganization { get; set; }

    /// <summary>
    ///   Disables access to this Project and schedules deletes for everything within,
    ///   when all items using it are deleted it will be removed.
    /// </summary>
    public bool DeleteFlag { get; set; } = false;
}