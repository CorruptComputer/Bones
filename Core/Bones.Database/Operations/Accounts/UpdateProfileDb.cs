using Bones.Database.DbSets.Accounts;

namespace Bones.Database.Operations.Accounts;

/// <inheritdoc />
public sealed class UpdateProfileDb(BonesDbContext dbContext) : IRequestHandler<UpdateProfileDb.Command, CommandResponse>
{
    /// <summary>
    ///   Sets the email confirmed date time on the user
    /// </summary>
    /// <param name="UserId"></param>
    /// <param name="DisplayName"></param>
    public sealed record Command(Guid UserId, string DisplayName) : IRequest<CommandResponse>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Command>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.DisplayName).NotEmpty();
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

        user.DisplayName = request.DisplayName;
        dbContext.Users.Update(user);

        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass();
    }
}
