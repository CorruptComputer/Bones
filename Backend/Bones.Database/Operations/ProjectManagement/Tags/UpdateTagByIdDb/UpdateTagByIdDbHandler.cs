using Bones.Database.DbSets.ProjectManagement;

namespace Bones.Database.Operations.ProjectManagement.Tags.UpdateTagByIdDb;

internal sealed class UpdateTagByIdDbHandler(BonesDbContext dbContext) : IRequestHandler<UpdateTagByIdDbCommand, CommandResponse>
{
    public async Task<CommandResponse> Handle(UpdateTagByIdDbCommand request, CancellationToken cancellationToken)
    {
        Tag? tag = await dbContext.Tags.FirstOrDefaultAsync(t => t.Id == request.TagId, cancellationToken);
        if (tag == null)
        {
            return CommandResponse.Fail("Invalid TagId.");
        }

        tag.Name = request.Name;

        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass();
    }
}