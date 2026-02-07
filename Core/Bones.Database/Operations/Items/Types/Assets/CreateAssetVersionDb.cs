using Bones.Database.DbSets.Items;
using Bones.Database.DbSets.Items.Assignments;
using Bones.Database.DbSets.Items.Fields;
using Bones.Database.DbSets.Items.Layouts;
using Bones.Database.DbSets.Items.Types;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Bones.Database.Operations.Items.Types.Assets;

/// <inheritdoc />
public sealed class CreateAssetVersionDb(BonesDbContext dbContext) : IRequestHandler<CreateAssetVersionDb.Command, CommandResponse>
{
    /// <summary>
    ///   DB Command for creating an ItemVersion.
    /// </summary>
    /// <param name="ItemId">Internal ID of the item</param>
    /// <param name="Title">The title to use for this version</param>
    /// <param name="LayoutVersionId">Internal ID of the layout version this item is using</param>
    /// <param name="Values">The values to use for this asset version</param>
    /// <param name="ActionDateTime"></param>
    public record Command(Guid ItemId, string Title, Guid LayoutVersionId, Dictionary<Guid, object?> Values, DateTimeOffset ActionDateTime) : IRequest<CommandResponse>;

    /// <inheritdoc />
    internal sealed class Validator : AbstractValidator<Command>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.ItemId).NotNull().NotEqual(Guid.Empty);
            RuleFor(x => x.Title).NotEmpty().MaximumLength(256);
            RuleFor(x => x.LayoutVersionId).NotNull().NotEqual(Guid.Empty);
            RuleFor(x => x.Values).NotNull().ChildRules(dict =>
            {
                dict.RuleForEach(x => x.Keys).NotEmpty();
            });
            RuleFor(x => x.ActionDateTime).NotNull().LessThanOrEqualTo(DateTimeOffset.UtcNow)
                .WithMessage("Action date time cannot be in the future");
        }
    }

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        Asset? asset = await dbContext.Assets
            .Include(item => item.Item)
            .FirstOrDefaultAsync(i => i.Id == request.ItemId, cancellationToken);

        if (asset is null)
        {
            return CommandResponse.Fail("Invalid Item ID.");
        }

        ItemLayoutVersion? layoutVersion = await dbContext.ItemLayoutVersions
            .Include(ilv => ilv.ItemLayoutFieldVersionLinks)
                .ThenInclude(ilfvl => ilfvl.ItemFieldVersion)
            .FirstOrDefaultAsync(ilv => ilv.Id == request.LayoutVersionId, cancellationToken);

        if (layoutVersion is null)
        {
            return CommandResponse.Fail("Invalid LayoutVersionId.");
        }

        if (request.Values.Count > layoutVersion.ItemLayoutFieldVersionLinks.Count)
        {
            return CommandResponse.Fail("Invalid values provided.");
        }

        Item? item = await dbContext.Items
            .Include(i => i.Versions)
                .ThenInclude(i => i.ItemAssignees)
            .FirstOrDefaultAsync(i => i.Id == asset.ItemId, cancellationToken);

        if (item is null)
        {
            return CommandResponse.Fail("Invalid Item ID.");
        }

        List<ItemAssignee> assignees = item.Current?.ItemAssignees ?? [];

        long version = ++item.CurrentVersion;

        EntityEntry<ItemVersion> newItemVersionEntry = dbContext.ItemVersions.Add(new()
        {
            ItemId = item.Id,
            Title = request.Title,
            Version = version,
            CreateDateTime = request.ActionDateTime,
            ItemLayoutVersionId = layoutVersion.Id,
            ItemAssignees = assignees
        });

        await dbContext.SaveChangesAsync(cancellationToken);

        foreach ((Guid fieldId, object? value) in request.Values)
        {
            ItemFieldVersion? itemFieldVersion = layoutVersion.ItemLayoutFieldVersionLinks.Find(f => f.ItemFieldVersionId == fieldId)?.ItemFieldVersion;
            if (itemFieldVersion == null)
            {
                return CommandResponse.Fail($"Invalid field name provided: {fieldId}");
            }

            ItemValue assetValue = new()
            {
                ItemFieldVersionId = itemFieldVersion.Id,
                ItemVersionId = newItemVersionEntry.Entity.Id,
                ItemFieldVersion = itemFieldVersion
            };

            if (value is not null)
            {
                bool valid = assetValue.TrySetValue(value);
                if (!valid)
                {
                    return CommandResponse.Fail($"Invalid value provided for '{Enum.GetName(itemFieldVersion.Type)}' field '{fieldId}': {value}");
                }
            }
            else if (itemFieldVersion.IsRequired)
            {
                return CommandResponse.Fail($"Field '{fieldId}' is required, but no value was provided.");
            }
            // else if its null and not required we can just skip doing anything else

            dbContext.ItemValues.Add(assetValue);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass(nameof(ItemVersion), newItemVersionEntry.Entity.Id);
    }
}