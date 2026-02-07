using System.Diagnostics.CodeAnalysis;
using Bones.Database.DbConsts;
using Bones.Database.DbSets.Accounts;
using Bones.Database.DbSets.Organizations;
using Bones.Shared.Backend.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bones.Database.DbSets.Projects;

/// <summary>
///   Model for the Projects.Projects table
/// </summary>
[Table(TableNames.ProjectManagement.Projects, Schema = SchemaNames.Projects)]
[PrimaryKey(nameof(Id))]
public class Project
{
    /// <summary>
    ///   Internal ID for the Tag
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    /// <summary>
    ///   The name of the project
    /// </summary>
    [MaxLength(512)]
    public required string Name { get; set; }

    /// <summary>
    ///   The type of owner for this project
    /// </summary>
    public OwnershipType OwnerType { get; set; }

    /// <summary>
    ///   The ID of the user that owns this project, if OwnerType is User
    /// </summary>
    public Guid? OwningUserId { get; set; }

    /// <summary>
    ///   The ID of the organization that owns this project, if OwnerType is Organization
    /// </summary>
    public Guid? OwningOrganizationId { get; set; }

    /// <summary>
    ///   Disables access to this Project and schedules deletes for everything within,
    ///   when all items using it are deleted it will be removed.
    /// </summary>
    public bool DeleteFlag { get; set; } = false;

    #region Navigational Properties
    /// <summary>
    ///   Navigational property for the user that owns this project, null if owned by an organization or not .Include()'d in the query
    /// </summary>
    public BonesUser? OwningUser { get; }

    /// <summary>
    ///   Navigational property for the organization that owns this project, null if owned by a user or not .Include()'d in the query
    /// </summary>
    public BonesOrganization? OwningOrganization { get; }

    /// <summary>
    ///   Navigational property for the initiatives associated with this project, empty if not .Include()'d in the query
    /// </summary>
    public List<Initiative> Initiatives { get; set; } = [];
    #endregion

    #region Non-Mapped Properties
    /// <summary>
    ///   Indicates whether this project is owned by a user. If true, OwningUserId is guaranteed to be not null.
    /// </summary>
    [NotMapped]
    [MemberNotNullWhen(true, nameof(OwningUserId))]
    public bool IsUserOwned => OwnerType == OwnershipType.User;

    /// <summary>
    ///   Indicates whether this project is owned by an organization. If true, OwningOrganizationId is guaranteed to be not null.
    /// </summary>
    [NotMapped]
    [MemberNotNullWhen(true, nameof(OwningOrganizationId))]
    public bool IsOrganizationOwned => OwnerType == OwnershipType.Organization;
    #endregion

    internal static void BuildTable(EntityTypeBuilder<Project> builder)
    {
        // Remove deleted items from being included in default queries
        builder.HasQueryFilter(x => !x.DeleteFlag);

        builder.HasOne(p => p.OwningUser)
               .WithMany()
               .HasForeignKey(p => p.OwningUserId);

        builder.HasOne(p => p.OwningOrganization)
               .WithMany()
               .HasForeignKey(p => p.OwningOrganizationId);

        builder.HasMany(p => p.Initiatives)
               .WithOne(i => i.Project)
               .HasForeignKey(i => i.ProjectId);
    }
}