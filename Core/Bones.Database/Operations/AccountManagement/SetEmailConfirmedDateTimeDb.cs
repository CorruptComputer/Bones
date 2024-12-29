using Bones.Database.DbSets.AccountManagement;

namespace Bones.Database.Operations.AccountManagement;

/// <summary>
///   Sets the email confirmed date time on the user
/// </summary>
/// <param name="User"></param>
/// <param name="ConfirmedDateTime"></param>
public sealed record SetEmailConfirmedDateTimeDbCommand(BonesUser User, DateTimeOffset ConfirmedDateTime) : IRequest<CommandResponse>;

internal sealed class SetEmailConfirmedDateTimeDbCommandValidator : AbstractValidator<SetEmailConfirmedDateTimeDbCommand>
{
    public SetEmailConfirmedDateTimeDbCommandValidator()
    {
        RuleFor(x => x.User).NotNull().Custom((user, ctx) => 
        {
            if (user.EmailConfirmed)
            {
                ctx.AddFailure("User", "User already has email confirmed");
            }
        });

        RuleFor(x => x.ConfirmedDateTime).NotNull();
    }
}

internal sealed class SetEmailConfirmedDateTimeDbHandler(BonesDbContext dbContext) : IRequestHandler<SetEmailConfirmedDateTimeDbCommand, CommandResponse>
{
    public async Task<CommandResponse> Handle(SetEmailConfirmedDateTimeDbCommand request, CancellationToken cancellationToken)
    {
        request.User.EmailConfirmed = true;
        request.User.EmailConfirmedDateTime = request.ConfirmedDateTime;
        dbContext.Users.Update(request.User);
        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass();
    }
}