using Bones.Database.DbSets.GenericItems.GenericItemLayouts;
using Bones.Database.DbSets.ProjectManagement;
using Bones.Shared.Backend.Enums;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Bones.Database.Operations.GenericItem.ItemLayouts.CreateItemLayoutDb;

internal class CreateItemLayoutDbHandler(BonesDbContext dbContext) : IRequestHandler<CreateItemLayoutDbCommand, CommandResponse>
{
    public async Task<CommandResponse> Handle(CreateItemLayoutDbCommand request, CancellationToken cancellationToken)
    {

        Project? project = await dbContext.Projects.FindAsync([request.ProjectId], cancellationToken);

        if (project == null)
        {
            return CommandResponse.Fail("Project does not exist");
        }

        GenericItemLayout newLayout = new()
        {
            Name = request.Name,
            ProjectId = project.Id,
            EnabledFor = Enum.GetValues<ItemLayoutUse>().ToList(),
        };

        EntityEntry<GenericItemLayout> added = await dbContext.ItemLayouts.AddAsync(newLayout, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass(added.Entity.Id);
    }
}