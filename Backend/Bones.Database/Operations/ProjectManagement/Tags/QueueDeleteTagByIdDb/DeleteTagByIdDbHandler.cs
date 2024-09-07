using Bones.Database.DbSets.ProjectManagement;

namespace Bones.Database.Operations.ProjectManagement.Tags.QueueDeleteTagByIdDb;

internal sealed class DeleteTagByIdDbHandler(BonesDbContext dbContext) : IRequestHandler<DeleteTagByIdDbCommand, CommandResponse>
{
    public async Task<CommandResponse> Handle(DeleteTagByIdDbCommand request, CancellationToken cancellationToken)
    {
        IQueryable<Tag> tag = dbContext.Tags.Where(p => p.Id == request.TagId);
        if (await tag.AnyAsync(cancellationToken))
        {
            return CommandResponse.Fail("Invalid TagId.");
        }

        await tag.ExecuteDeleteAsync(cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass();
    }
}