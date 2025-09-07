using Bones.Database.DbConsts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bones.Database.DbSets.AccountManagement;

/// <summary>
///   Model for the AccountManagement.BonesRoleClaims table.
/// </summary>
[Table(TableNames.AccountManagement.BonesRoleClaims, Schema = SchemaNames.AccountManagement)]
public class BonesRoleClaim : IdentityRoleClaim<Guid>
{
    /// <summary>
    ///   Needed to override the default table name and schema that <see cref="IdentityRoleClaim{Guid}" /> uses.
    ///   Seems the attribute is ignored by that, still keeping it there for consistency though.
    /// </summary>
    /// <param name="builder"></param>
    internal static void BuildTable(EntityTypeBuilder<BonesRoleClaim> builder)
    {
        builder.ToTable(TableNames.AccountManagement.BonesRoleClaims, SchemaNames.AccountManagement);
    }
}