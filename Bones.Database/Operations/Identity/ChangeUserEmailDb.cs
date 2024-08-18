using Bones.Database.DbSets.Identity;

namespace Bones.Database.Operations.Identity;

public sealed class ChangeUserEmailDb(BonesDbContext dbContext, ISender sender) : IRequestHandler<ChangeUserEmailDb.Command, CommandResponse>
{
    /// <summary>
    ///     DB Command for updating the email address on a user.
    /// </summary>
    /// <param name="UserId">The ID of the User to update</param>
    /// <param name="Email">The new email address</param>
    public record Command(Guid UserId, string Email) : IValidatableRequest<CommandResponse>
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

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        CommandResponse emailAvailable =
            await sender.Send(new EmailAvailableForUseDb.Command(request.Email), cancellationToken);
        if (!emailAvailable.Success)
        {
            return emailAvailable;
        }

        BonesUser? acct =
            await dbContext.Users.FirstOrDefaultAsync(a => a.Id == request.UserId, cancellationToken);

        if (acct == null)
        {
            return new()
            {
                Success = false,
                FailureReason = "Failed to find the specified User."
            };
        }

        acct.Email = request.Email;
        acct.EmailConfirmed = false;
        acct.EmailConfirmedDateTime = null;

        await dbContext.SaveChangesAsync(cancellationToken);

        return new()
        {
            Success = true
        };
    }
}