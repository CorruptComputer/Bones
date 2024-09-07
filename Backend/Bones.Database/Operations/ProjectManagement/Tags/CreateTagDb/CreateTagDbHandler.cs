using Bones.Database.DbSets.ProjectManagement;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Bones.Database.Operations.ProjectManagement.Tags.CreateTagDb;

internal sealed class CreateTagDbHandler(BonesDbContext dbContext) : IRequestHandler<CreateTagDbCommand, CommandResponse>
{
    public async Task<CommandResponse> Handle(CreateTagDbCommand request, CancellationToken cancellationToken)
    {
        EntityEntry<Tag> created = await dbContext.Tags.AddAsync(new()
        {
            Name = request.Name
        }, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass(created.Entity.Id);
    }
}