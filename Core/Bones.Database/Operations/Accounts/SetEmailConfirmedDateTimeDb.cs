using Bones.Database.DbSets.Accounts;

namespace Bones.Database.Operations.Accounts;

/// <inheritdoc />
public sealed class SetEmailConfirmedDateTimeDb(BonesDbContext dbContext) : IRequestHandler<SetEmailConfirmedDateTimeDb.Command, CommandResponse>
{
    /// <summary>
    ///   Sets the email confirmed date time on the user
    /// </summary>
    /// <param name="UserId"></param>
    /// <param name="ConfirmedDateTime"></param>
    public sealed record Command(Guid UserId, DateTimeOffset ConfirmedDateTime) : IRequest<CommandResponse>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Command>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.ConfirmedDateTime).NotNull();
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

        user.EmailConfirmed = true;
        user.EmailConfirmedDateTime = request.ConfirmedDateTime;
        dbContext.Users.Update(user);

        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass();
    }
}