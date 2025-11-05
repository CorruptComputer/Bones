using Bones.Database.DbSets.Items;
using Bones.Database.DbSets.WorkItemManagement;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Bones.Database.Operations.WorkItemManagement.WorkItems;

/// <inheritdoc />
public sealed class CreateWorkItemVersionDb(BonesDbContext dbContext) : IRequestHandler<CreateWorkItemVersionDb.Command, CommandResponse>
{
    /// <summary>
    ///   DB Command for creating an ItemVersion.
    /// </summary>
    /// <param name="WorkItemId">Internal ID of the item</param>
    /// <param name="Title">The title to use for this version</param>
    /// <param name="WorkItemLayoutVersionId">Internal ID of the layout version this item is using</param>
    /// <param name="Values">The values to use for this work item version</param>
    /// <param name="ActionDateTime"></param>
    public record Command(Guid WorkItemId, string Title, Guid WorkItemLayoutVersionId, Dictionary<Guid, object?> Values, DateTimeOffset ActionDateTime) : IRequest<CommandResponse>;

    /// <inheritdoc />
    internal sealed class Validator : AbstractValidator<Command>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.WorkItemId).NotNull().NotEqual(Guid.Empty);
            RuleFor(x => x.Title).NotNull().NotEmpty().MaximumLength(256);
            RuleFor(x => x.WorkItemLayoutVersionId).NotNull().NotEqual(Guid.Empty);
            RuleFor(x => x.Values).NotNull().ChildRules(dict =>
            {
                dict.RuleForEach(x => x.Keys).NotNull().NotEmpty();
            });
            RuleFor(x => x.ActionDateTime).NotNull().LessThanOrEqualTo(DateTimeOffset.UtcNow)
                .WithMessage("Action date time cannot be in the future");
        }
    }

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        WorkItem? workItem = await dbContext.WorkItems
            .Include(item => item.Item)
            .FirstOrDefaultAsync(i => i.Id == request.WorkItemId, cancellationToken);

        if (workItem == null)
        {
            return CommandResponse.Fail("Invalid WorkItem ID.");
        }

        ItemLayoutVersion? layoutVersion = await dbContext.ItemLayoutVersions
            .Include(lv => lv.FieldLinks)
            .ThenInclude(fl => fl.FieldVersion)
            .FirstOrDefaultAsync(lv => lv.Id == request.WorkItemLayoutVersionId, cancellationToken);

        if (layoutVersion == null)
        {
            return CommandResponse.Fail("Invalid LayoutVersionId.");
        }

        if (request.Values.Count > layoutVersion.FieldLinks.Count)
        {
            return CommandResponse.Fail("Invalid values provided.");
        }

        List<ItemValue> values = [];
        foreach ((Guid fieldId, object? value) in request.Values)
        {
            ItemFieldVersion? field = layoutVersion.FieldLinks.Find(f => f.FieldVersion.Id == fieldId)?.FieldVersion;
            if (field == null)
            {
                return CommandResponse.Fail($"Invalid field name provided: {fieldId}");
            }

            ItemValue workItemValue = new()
            {
                Field = field,

            };

            if (value is not null)
            {
                bool valid = workItemValue.TrySetValue(value);
                if (!valid)
                {
                    return CommandResponse.Fail($"Invalid value provided for '{Enum.GetName(field.Type)}' field '{fieldId}': {value}");
                }
            }
            else if (field.IsRequired)
            {
                return CommandResponse.Fail($"Field '{fieldId}' is required, but no value was provided.");
            }
            // else if its null and not required we can just skip doing anything else

            values.Add(workItemValue);
        }

        int version = ++workItem.Item.CurrentVersion;

        workItem.Item.Versions.Add(new()
        {
            Item = workItem.Item,
            Title = request.Title,
            Version = version,
            CreateDateTime = request.ActionDateTime,
            ItemLayoutVersion = layoutVersion,
            Values = values
        });

        EntityEntry<WorkItem> updated = dbContext.WorkItems.Update(workItem);

        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass(nameof(ItemVersion), updated.Entity.Item.Versions.FirstOrDefault(v => v.Version == version)?.Id);
    }
}