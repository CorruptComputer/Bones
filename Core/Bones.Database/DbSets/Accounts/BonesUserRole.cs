using Bones.Database.DbConsts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bones.Database.DbSets.Accounts;

/// <summary>
///   Model for the Accounts.BonesUserRoles table.
/// </summary>
[Table(TableNames.Accounts.BonesUserRoles, Schema = SchemaNames.Accounts)]
public class BonesUserRole : IdentityUserRole<Guid>
{
    /// <summary>
    ///   Needed to override the default table name and schema that <see cref="IdentityUserRole{Guid}" /> uses.
    ///   Seems the attribute is ignored by that, still keeping it there for consistency though.
    /// </summary>
    /// <param name="builder"></param>
    internal static void BuildTable(EntityTypeBuilder<BonesUserRole> builder)
    {
        builder.ToTable(TableNames.Accounts.BonesUserRoles, SchemaNames.Accounts);
    }
}