namespace Bones.Database.DbSets.Identity;

/// <summary>
///     Model for the Identity.UserEmailVerifications table
/// </summary>
[Table("UserEmailVerifications", Schema = "Identity")]
[PrimaryKey(nameof(Id))]
public class UserEmailVerification
{
    /// <summary>
    ///     Internal ID for the User Email Verification
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    /// <summary>
    ///   The user 
    /// </summary>
    public required BonesUser User { get; set; }

    /// <summary>
    ///     Verification token
    /// </summary>
    // TODO: Might want to use something other than a Guid for this at some point
    public required Guid Token { get; set; }

    /// <summary>
    ///     Time this verification request was created
    /// </summary>
    public required DateTimeOffset CreateDateTime { get; set; }

    /// <summary>
    ///     Time this verification is valid until
    /// </summary>
    public required DateTimeOffset ValidUntilDateTime { get; set; }
}