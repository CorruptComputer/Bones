using Bones.Database.DbSets.Identity;

namespace Bones.Database.Operations.Identity;

public sealed class EmailAvailableForUseDb(BonesDbContext dbContext) : IRequestHandler<EmailAvailableForUseDb.Command, CommandResponse>
{
    /// <summary>
    ///     DB Command for checking if an email is available to use.
    /// </summary>
    /// <param name="Email">Email address to check</param>
    public record Command(string Email) : IValidatableRequest<CommandResponse>
    {
        /// <inheritdoc />
        public (bool valid, string? invalidReason) IsRequestValid()
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                return (false, "");
            }

            return (true, null);
        }
    }


    public async Task<CommandResponse> Handle(Command request,
        CancellationToken cancellationToken)
    {
        BonesUser? User =
            await dbContext.Users.FirstOrDefaultAsync(a => a.Email == request.Email, cancellationToken);

        if (User != null)
        {
            return new()
            {
                Success = false,
                FailureReason = "Email already in use on an existing User."
            };
        }

        return new()
        {
            Success = true
        };
    }
}