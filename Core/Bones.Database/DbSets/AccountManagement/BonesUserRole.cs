using Bones.Database.DbConsts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bones.Database.DbSets.AccountManagement;

/// <summary>
///     Model for the AccountManagement.BonesUserRoles table.
/// </summary>
[Table(TableNames.AccountManagement.BonesUserRoles, Schema = SchemaNames.AccountManagement)]
public class BonesUserRole : IdentityUserRole<Guid>
{
    /// <summary>
    ///   Needed to override the default table name and schema that <see cref="IdentityUserRole{Guid}" /> uses.
    ///   Seems the attribute is ignored by that, still keeping it there for consistency though.
    /// </summary>
    /// <param name="builder"></param>
    internal static void BuildTable(EntityTypeBuilder<BonesUserRole> builder)
    {
        builder.ToTable(TableNames.AccountManagement.BonesUserRoles, SchemaNames.AccountManagement);
    }
}