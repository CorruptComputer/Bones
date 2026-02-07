using Bones.Database.DbConsts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bones.Database.DbSets.Accounts;

/// <summary>
///   Model for the Accounts.BonesUserLogins table.
/// </summary>
[Table(TableNames.Accounts.BonesUserLogins, Schema = SchemaNames.Accounts)]
public class BonesUserLogin : IdentityUserLogin<Guid>
{
    /// <summary>
    ///   Needed to override the default table name and schema that <see cref="IdentityUserLogin{Guid}" /> uses.
    ///   Seems the attribute is ignored by that, still keeping it there for consistency though.
    /// </summary>
    /// <param name="builder"></param>
    internal static void BuildTable(EntityTypeBuilder<BonesUserLogin> builder)
    {
        builder.ToTable(TableNames.Accounts.BonesUserLogins, SchemaNames.Accounts);
    }
}