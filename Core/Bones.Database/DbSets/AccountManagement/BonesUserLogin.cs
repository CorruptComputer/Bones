using Bones.Database.DbConsts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bones.Database.DbSets.AccountManagement;

/// <summary>
///     Model for the AccountManagement.BonesUserLogins table.
/// </summary>
[Table(TableNames.AccountManagement.BonesUserLogins, Schema = SchemaNames.AccountManagement)]
public class BonesUserLogin : IdentityUserLogin<Guid>
{
    /// <summary>
    ///   Needed to override the default table name and schema that <see cref="IdentityUserLogin{Guid}" /> uses.
    ///   Seems the attribute is ignored by that, still keeping it there for consistency though.
    /// </summary>
    /// <param name="builder"></param>
    internal static void BuildTable(EntityTypeBuilder<BonesUserLogin> builder)
    {
        builder.ToTable(TableNames.AccountManagement.BonesUserLogins, SchemaNames.AccountManagement);
    }
}