using Bones.Database.DbSets.ProjectManagement;

namespace Bones.Database.Operations.ProjectManagement.Initiatives.UpdateInitiativeByIdDb;

internal sealed class UpdateInitiativeByIdDbHandler(BonesDbContext dbContext) : IRequestHandler<UpdateInitiativeByIdDbCommand, CommandResponse>
{
    public async Task<CommandResponse> Handle(UpdateInitiativeByIdDbCommand request, CancellationToken cancellationToken)
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