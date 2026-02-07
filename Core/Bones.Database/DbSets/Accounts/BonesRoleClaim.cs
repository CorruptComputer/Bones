using Bones.Database.DbConsts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bones.Database.DbSets.Accounts;

/// <summary>
///   Model for the Accounts.BonesRoleClaims table.
/// </summary>
[Table(TableNames.Accounts.BonesRoleClaims, Schema = SchemaNames.Accounts)]
public class BonesRoleClaim : IdentityRoleClaim<Guid>
{
    /// <summary>
    ///   Needed to override the default table name and schema that <see cref="IdentityRoleClaim{Guid}" /> uses.
    ///   Seems the attribute is ignored by that, still keeping it there for consistency though.
    /// </summary>
    /// <param name="builder"></param>
    internal static void BuildTable(EntityTypeBuilder<BonesRoleClaim> builder)
    {
        builder.ToTable(TableNames.Accounts.BonesRoleClaims, SchemaNames.Accounts);
    }
}