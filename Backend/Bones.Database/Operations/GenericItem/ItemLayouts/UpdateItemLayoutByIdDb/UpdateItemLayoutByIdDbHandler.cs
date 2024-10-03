using Bones.Database.DbSets.GenericItems.ItemLayouts;

namespace Bones.Database.Operations.GenericItem.ItemLayouts.UpdateItemLayoutByIdDb;

internal class UpdateItemLayoutByIdDbHandler(BonesDbContext dbContext) : IRequestHandler<UpdateItemLayoutByIdDbCommand, CommandResponse>
{
    public async Task<CommandResponse> Handle(UpdateItemLayoutByIdDbCommand request, CancellationToken cancellationToken)
    {
        ItemLayout? layout = await dbContext.ItemLayouts.FindAsync([request.ItemLayoutId], cancellationToken);

        if (layout == null)
        {
            return CommandResponse.Fail("ItemLayout not found");
        }

        layout.Name = request.NewName;

        await dbContext.SaveChangesAsync(cancellationToken);
        return CommandResponse.Pass();
    }
}