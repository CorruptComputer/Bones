using Bones.Database.DbSets.AssetManagement;
using Bones.Database.DbSets.Items;

namespace Bones.Database.Operations.AssetManagement;

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
            .ThenInclude(item => item.Versions)
            .FirstOrDefaultAsync(p => p.Id == request.AssetId, cancellationToken);

        if (asset == null)
        {
            return CommandResponse.Fail("Invalid AssetId.");
        }

        foreach (ItemVersion version in asset.Item.Versions)
        {
            await sender.Send(new QueueDeleteAssetVersionByIdDb.Command(version.Id), cancellationToken);
        }

        asset.DeleteFlag = true;

        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass();
    }
}