using Bones.Database.DbSets.Items;
using Bones.Database.DbSets.Items.Types;

namespace Bones.Database.Operations.Items.Types.Assets;

/// <inheritdoc />
public sealed class QueueDeleteAssetByIdDb(BonesDbContext dbContext, ISender sender) : IRequestHandler<QueueDeleteAssetByIdDb.Command, CommandResponse>
{
    /// <summary>
    ///   DB Command for deleting an Asset.
    /// </summary>
    /// <param name="AssetId">Internal ID of the item</param>
    public record Command(Guid AssetId) : IRequest<CommandResponse>;

    /// <inheritdoc />
    public class Validator : AbstractValidator<Command>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.AssetId).NotNull().NotEqual(Guid.Empty);
        }
    }

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        Asset? asset = await dbContext.Assets
            .Include(item => item.Item)
            .FirstOrDefaultAsync(p => p.Id == request.AssetId, cancellationToken);

        if (asset == null || asset.Item == null)
        {
            return CommandResponse.Fail("Invalid AssetId.");
        }

        List<ItemVersion> versions = await dbContext.ItemVersions
            .Where(iv => iv.ItemId == asset.ItemId)
            .ToListAsync(cancellationToken);

        foreach (ItemVersion version in versions)
        {
            await sender.Send(new QueueDeleteAssetVersionByIdDb.Command(version.Id), cancellationToken);
        }

        asset.DeleteFlag = true;
        asset.Item.DeleteFlag = true;

        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass();
    }
}