using Bones.Database.DbSets.ProjectManagement;
using Bones.Database.Operations.ProjectManagement.Initiatives.QueueDeleteInitiativeByIdDb;

namespace Bones.Database.Operations.ProjectManagement.Projects.QueueDeleteProjectByIdDb;

internal sealed class QueueDeleteProjectByIdDbHandler(BonesDbContext dbContext, ISender sender) : IRequestHandler<QueueDeleteProjectByIdDbCommand, CommandResponse>
{
    /// <inheritdoc />
    public async Task<CommandResponse> Handle(QueueDeleteProjectByIdDbCommand request, CancellationToken cancellationToken)
    {
        Project? project = await dbContext.Projects.Include(project => project.Initiatives).FirstOrDefaultAsync(p => p.Id == request.ProjectId, cancellationToken);
        if (project == null)
        {
            return CommandResponse.Fail("Invalid ProjectId.");
        }

        foreach (Initiative initiative in project.Initiatives)
        {
            await sender.Send(new QueueDeleteInitiativeByIdDbCommand(initiative.Id), cancellationToken);
        }

        project.DeleteFlag = true;

        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass();
    }
}