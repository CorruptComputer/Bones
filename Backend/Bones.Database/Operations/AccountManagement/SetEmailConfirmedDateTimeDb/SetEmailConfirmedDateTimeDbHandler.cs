namespace Bones.Database.Operations.AccountManagement.SetEmailConfirmedDateTimeDb;

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