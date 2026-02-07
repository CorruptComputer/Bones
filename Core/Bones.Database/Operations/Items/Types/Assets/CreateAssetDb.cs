using Bones.Database.DbSets.Items;
using Bones.Database.DbSets.Items.Layouts;
using Bones.Database.DbSets.Items.Types;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Bones.Database.Operations.Items.Types.Assets;

/// <inheritdoc />
public sealed class CreateAssetDb(BonesDbContext dbContext) : IRequestHandler<CreateAssetDb.Command, CommandResponse>
{
    /// <summary>
    ///   DB Command for creating an Asset.
    /// </summary>
    /// <param name="ItemLayoutId">Internal ID of the layout this item is using</param>
    /// <param name="ActionDateTime"></param>
    public sealed record Command(Guid ItemLayoutId, DateTimeOffset ActionDateTime) : IRequest<CommandResponse>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Command>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.ItemLayoutId).NotNull().NotEqual(Guid.Empty);
            RuleFor(x => x.ActionDateTime).NotNull().NotEqual(default(DateTimeOffset));
            RuleFor(x => x.ActionDateTime).LessThanOrEqualTo(DateTimeOffset.UtcNow)
                .WithMessage("ActionDateTime cannot be in the future.");
        }
    }

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        ItemLayout? itemLayout = await dbContext.ItemLayouts.Include(l => l.Project).FirstOrDefaultAsync(l => l.Id == request.ItemLayoutId, cancellationToken);
        if (itemLayout == null)
        {
            return CommandResponse.Fail("Invalid ItemLayout ID.");
        }

        EntityEntry<Item> createdItem = await dbContext.Items.AddAsync(new()
        {
            FriendlyId = $"{itemLayout.FriendlyIdPrefix}-{itemLayout.FriendlyIdNonce++}",
            ProjectId = itemLayout.ProjectId,
            ItemLayoutId = itemLayout.Id
        }, cancellationToken);

        // ID of the Item is DB generated, need to save it to the DB before its ID is available
        await dbContext.SaveChangesAsync(cancellationToken);

        EntityEntry<Asset> created = await dbContext.Assets.AddAsync(new()
        {
            ProjectId = itemLayout.ProjectId,
            ItemId = createdItem.Entity.Id
        }, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass(nameof(Asset), created.Entity.Id);
    }
}