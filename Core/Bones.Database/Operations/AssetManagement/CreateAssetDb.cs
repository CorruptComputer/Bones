using Bones.Database.DbSets.AssetManagement;
using Bones.Database.DbSets.Items;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Bones.Database.Operations.AssetManagement;

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

        EntityEntry<Asset> created = await dbContext.Assets.AddAsync(new()
        {
            Project = itemLayout.Project,
            Item = new()
            {
                FriendlyId = $"{itemLayout.FriendlyIdPrefix}-{itemLayout.FriendlyIdNonce++}",
                Project = itemLayout.Project,
                ItemLayout = itemLayout
            }
        }, cancellationToken);

        dbContext.ItemLayouts.Update(itemLayout);

        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass(nameof(Asset), created.Entity.Id);
    }
}