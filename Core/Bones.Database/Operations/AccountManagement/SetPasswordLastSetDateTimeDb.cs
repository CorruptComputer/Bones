using Bones.Database.DbSets.AccountManagement;

namespace Bones.Database.Operations.AccountManagement;

/// <inheritdoc />
public sealed class SetPasswordLastSetDateTimeDb(BonesDbContext dbContext) : IRequestHandler<SetPasswordLastSetDateTimeDb.Command, CommandResponse>
{
    /// <summary>
    ///   Sets the email confirmed date time on the user
    /// </summary>
    /// <param name="UserId"></param>
    /// <param name="LastSetDateTime"></param>
    public sealed record Command(Guid UserId, DateTimeOffset LastSetDateTime) : IRequest<CommandResponse>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Command>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.LastSetDateTime).NotNull();
        }
    }

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        BonesUser? user = await dbContext.Users
            .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);

        if (user is null)
        {
            return CommandResponse.Fail("User not found");
        }

        user.PasswordLastSetDateTime = request.LastSetDateTime;
        dbContext.Users.Update(user);

        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass();
    }
}