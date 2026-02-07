using Bones.Database.DbConsts;
using Bones.Database.DbSets.Accounts;
using Bones.Database.DbSets.Projects;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bones.Database.DbSets.Organizations;

/// <summary>
///   Model for the Organizations.BonesOrganizations table
/// </summary>
[Table(TableNames.OrganizationManagement.BonesOrganizations, Schema = SchemaNames.Organizations)]
[PrimaryKey(nameof(Id))]
public class BonesOrganization
{
    /// <summary>
    ///   Internal ID for the BonesOrganization
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    /// <summary>
    ///   The name of this organization
    /// </summary>
    [MaxLength(512)]
    public required string Name { get; set; }

    #region Navigational Properties
    /// <summary>
    ///   Navigational property for the roles associated with this organization, empty if not .Include()'d in the query
    /// </summary>
    public List<BonesRole> Roles { get; set; } = [];

    /// <summary>
    ///   Navigational property for the projects associated with this organization, empty if not .Include()'d in the query
    /// </summary>
    public List<Project> Projects { get; set; } = [];
    #endregion


    internal static void BuildTable(EntityTypeBuilder<BonesOrganization> builder)
    {
        builder.HasMany(bo => bo.Roles)
               .WithOne(r => r.Organization)
               .HasForeignKey(r => r.OrganizationId);

        builder.HasMany(bo => bo.Projects)
               .WithOne(p => p.OwningOrganization)
               .HasForeignKey(p => p.OwningOrganizationId);
    }
}