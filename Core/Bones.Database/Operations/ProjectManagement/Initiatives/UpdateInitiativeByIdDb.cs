using Bones.Database.DbSets.ProjectManagement;

namespace Bones.Database.Operations.ProjectManagement.Initiatives;

/// <inheritdoc />
public sealed class UpdateInitiativeByIdDb(BonesDbContext dbContext) : IRequestHandler<UpdateInitiativeByIdDb.Command, CommandResponse>
{
    /// <summary>
    ///   DB Command for updating an initiative
    /// </summary>
    /// <param name="InitiativeId"></param>
    /// <param name="NewName"></param>
    public sealed record Command(Guid InitiativeId, string NewName) : IRequest<CommandResponse>;

    /// <inheritdoc />
    public class Validator : AbstractValidator<Command>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.InitiativeId).NotNull().NotEqual(Guid.Empty);
            RuleFor(x => x.NewName).NotNull().NotEmpty();
        }
    }

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        Initiative? initiative = await dbContext.Initiatives.FirstOrDefaultAsync(i => i.Id == request.InitiativeId, cancellationToken);
        if (initiative == null)
        {
            return CommandResponse.Fail("Invalid InitiativeId.");
        }

        initiative.Name = request.NewName;

        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass();
    }
}