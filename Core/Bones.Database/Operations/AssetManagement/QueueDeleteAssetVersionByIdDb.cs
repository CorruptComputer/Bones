using Bones.Database.DbSets.Items;

namespace Bones.Database.Operations.AssetManagement;

/// <inheritdoc />
public sealed class QueueDeleteAssetVersionByIdDb(BonesDbContext dbContext) : IRequestHandler<QueueDeleteAssetVersionByIdDb.Command, CommandResponse>
{
    /// <summary>
    ///   DB Command for deleting an ItemVersion.
    /// </summary>
    /// <param name="AssetVersionId">Internal ID of the item version</param>
    public record Command(Guid AssetVersionId) : IRequest<CommandResponse>;

    /// <inheritdoc />
    public class Validator : AbstractValidator<Command>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.AssetVersionId).NotNull().NotEqual(Guid.Empty);
        }
    }

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        ItemVersion? assetVersion = await dbContext.ItemVersions
            .Include(itemVersion => itemVersion.ItemValues)
            .FirstOrDefaultAsync(p => p.Id == request.AssetVersionId, cancellationToken);

        if (assetVersion == null)
        {
            return CommandResponse.Fail("Invalid AssetVersionId.");
        }

        foreach (ItemValue value in assetVersion.ItemValues)
        {
            value.DeleteFlag = true;
        }

        assetVersion.DeleteFlag = true;

        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass();
    }
}
