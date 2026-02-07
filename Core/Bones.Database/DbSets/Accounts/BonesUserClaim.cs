using Bones.Database.DbConsts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bones.Database.DbSets.Accounts;

/// <summary>
///   Model for the Accounts.BonesUserClaims table.
/// </summary>
[Table(TableNames.Accounts.BonesUserClaims, Schema = SchemaNames.Accounts)]
public class BonesUserClaim : IdentityUserClaim<Guid>
{
    /// <summary>
    ///   Needed to override the default table name and schema that <see cref="IdentityUserClaim{Guid}" /> uses.
    ///   Seems the attribute is ignored by that, still keeping it there for consistency though.
    /// </summary>
    /// <param name="builder"></param>
    internal static void BuildTable(EntityTypeBuilder<BonesUserClaim> builder)
    {
        builder.ToTable(TableNames.Accounts.BonesUserClaims, SchemaNames.Accounts);
    }
}