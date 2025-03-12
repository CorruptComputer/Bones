using Bones.Database.DbSets.GenericItems;
using Bones.Database.DbSets.WorkItemManagement;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Bones.Database.Operations.WorkItemManagement.WorkItems;

/// <inheritdoc />
public sealed class CreateWorkItemVersionDb(BonesDbContext dbContext) : IRequestHandler<CreateWorkItemVersionDb.Command, CommandResponse>
{
    /// <summary>
    ///     DB Command for creating an ItemVersion.
    /// </summary>
    /// <param name="WorkItemId">Internal ID of the item</param>
    /// <param name="WorkItemLayoutVersionId">Internal ID of the layout version this item is using</param>
    /// <param name="Values">The values to use for this work item version</param>
    public record Command(Guid WorkItemId, Guid WorkItemLayoutVersionId, Dictionary<string, object> Values) : IRequest<CommandResponse>;

    /// <inheritdoc />
    internal sealed class Validator : AbstractValidator<Command>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.WorkItemId).NotNull().NotEqual(Guid.Empty);
            RuleFor(x => x.WorkItemLayoutVersionId).NotNull().NotEqual(Guid.Empty);
            RuleFor(x => x.Values).NotNull().ChildRules(dict =>
            {
                dict.RuleForEach(x => x.Keys).NotNull().NotEmpty();
            });
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

        GenericItemLayoutVersion? layoutVersion = await dbContext.ItemLayoutVersions
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

        List<GenericItemValue> values = [];

        foreach ((string? key, object? value) in request.Values)
        {
            GenericItemFieldVersion? field = layoutVersion.FieldLinks.Find(f => f.FieldVersion.Name == key)?.FieldVersion;
            if (field == null)
            {
                return CommandResponse.Fail($"Invalid field name provided: {key}");
            }

            GenericItemValue workGenericItemValue = new() { Field = field };
            bool valid = workGenericItemValue.TrySetValue(value);
            if (!valid)
            {
                return CommandResponse.Fail($"Invalid value provided for '{Enum.GetName(field.Type)}' field '{key}': {value}");
            }

            values.Add(workGenericItemValue);
        }

        workItem.Item.Versions.Add(new()
        {
            Item = workItem.Item,
            Version = ++workItem.Item.CurrentVersion,
            CreateDateTime = DateTimeOffset.Now,
            GenericItemLayoutVersion = layoutVersion,
            Values = values
        });

        EntityEntry<WorkItem> updated = dbContext.WorkItems.Update(workItem);

        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass(updated.Entity.Item.Versions[updated.Entity.Item.CurrentVersion].Id);
    }
}