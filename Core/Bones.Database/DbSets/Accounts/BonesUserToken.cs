using Bones.Database.DbConsts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bones.Database.DbSets.Accounts;

/// <summary>
///   Model for the Accounts.BonesUserTokens table.
/// </summary>
[Table(TableNames.Accounts.BonesUserTokens, Schema = SchemaNames.Accounts)]
public class BonesUserToken : IdentityUserToken<Guid>
{
    /// <summary>
    ///   Needed to override the default table name and schema that <see cref="IdentityUserToken{Guid}" /> uses.
    ///   Seems the attribute is ignored by that, still keeping it there for consistency though.
    /// </summary>
    /// <param name="builder"></param>
    internal static void BuildTable(EntityTypeBuilder<BonesUserToken> builder)
    {
        builder.ToTable(TableNames.Accounts.BonesUserTokens, SchemaNames.Accounts);
    }
}