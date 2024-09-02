using Bones.Database.DbSets.AssetManagement.AssetLayouts;

namespace Bones.Database.Operations.AssetManagement.AssetLayouts.UpdateAssetLayoutByIdDb;

internal class UpdateAssetLayoutByIdDbHandler(BonesDbContext dbContext) : IRequestHandler<UpdateAssetLayoutByIdDbCommand, CommandResponse>
{
    public async Task<CommandResponse> Handle(UpdateAssetLayoutByIdDbCommand request, CancellationToken cancellationToken)
    {
        AssetLayout? layout = await dbContext.AssetLayouts.FindAsync([request.AssetLayoutId], cancellationToken);
        
        if (layout == null)
        {
            return CommandResponse.Fail("Asset Layout not found");
        }
        
        layout.Name = request.NewName;
        
        await dbContext.SaveChangesAsync(cancellationToken);
        return CommandResponse.Pass();
    }
}