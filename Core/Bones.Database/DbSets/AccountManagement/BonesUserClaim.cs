using Bones.Database.DbConsts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bones.Database.DbSets.AccountManagement;

/// <summary>
///   Model for the AccountManagement.BonesUserClaims table.
/// </summary>
[Table(TableNames.AccountManagement.BonesUserClaims, Schema = SchemaNames.AccountManagement)]
public class BonesUserClaim : IdentityUserClaim<Guid>
{
    /// <summary>
    ///   Needed to override the default table name and schema that <see cref="IdentityUserClaim{Guid}" /> uses.
    ///   Seems the attribute is ignored by that, still keeping it there for consistency though.
    /// </summary>
    /// <param name="builder"></param>
    internal static void BuildTable(EntityTypeBuilder<BonesUserClaim> builder)
    {
        builder.ToTable(TableNames.AccountManagement.BonesUserClaims, SchemaNames.AccountManagement);
    }
}