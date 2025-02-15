using Bones.Database.DbConsts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bones.Database.DbSets.AccountManagement;

/// <summary>
///     Model for the AccountManagement.BonesUserTokens table.
/// </summary>
[Table(TableNames.AccountManagement.BonesUserTokens, Schema = SchemaNames.AccountManagement)]
public class BonesUserToken : IdentityUserToken<Guid>
{
    /// <summary>
    ///   Needed to override the default table name and schema that <see cref="IdentityUserToken{Guid}" /> uses.
    ///   Seems the attribute is ignored by that, still keeping it there for consistency though.
    /// </summary>
    /// <param name="builder"></param>
    internal static void BuildTable(EntityTypeBuilder<BonesUserToken> builder)
    {
        builder.ToTable(TableNames.AccountManagement.BonesUserTokens, SchemaNames.AccountManagement);
    }
}