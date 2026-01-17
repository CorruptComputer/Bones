using Bones.Database.DbSets.Items;
using Bones.Database.DbSets.TaskManagement;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Bones.Database.Operations.TaskManagement.Tasks;

/// <inheritdoc />
public sealed class CreateTaskVersionDb(BonesDbContext dbContext) : IRequestHandler<CreateTaskVersionDb.Command, CommandResponse>
{
    /// <summary>
    ///   DB Command for creating a new ItemVersion for the task.
    /// </summary>
    /// <param name="TaskId">Internal ID of the task</param>
    /// <param name="Title">The title to use for this version</param>
    /// <param name="TaskLayoutVersionId">Internal ID of the layout version this item is using</param>
    /// <param name="Values">The values to use for this task version</param>
    /// <param name="ActionDateTime"></param>
    public record Command(Guid TaskId, string Title, Guid TaskLayoutVersionId, Dictionary<Guid, object?> Values, DateTimeOffset ActionDateTime) : IRequest<CommandResponse>;

    /// <inheritdoc />
    internal sealed class Validator : AbstractValidator<Command>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.TaskId).NotNull().NotEqual(Guid.Empty);
            RuleFor(x => x.Title).NotEmpty().MaximumLength(256);
            RuleFor(x => x.TaskLayoutVersionId).NotNull().NotEqual(Guid.Empty);
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
        BonesTask? task = await dbContext.Tasks
            .Include(t => t.Item)
                .ThenInclude(i => i!.Versions)
                .ThenInclude(v => v.ItemAssignees)
            .FirstOrDefaultAsync(t => t.Id == request.TaskId, cancellationToken);

        if (task is null || task.Item is null)
        {
            return CommandResponse.Fail("Invalid Item ID.");
        }

        ItemLayoutVersion? layoutVersion = await dbContext.ItemLayoutVersions
            .Include(lv => lv.ItemLayoutFieldVersionLinks)
                .ThenInclude(fl => fl.ItemFieldVersion)
            .FirstOrDefaultAsync(lv => lv.Id == request.TaskLayoutVersionId, cancellationToken);

        if (layoutVersion == null)
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
            .FirstOrDefaultAsync(i => i.Id == task.ItemId, cancellationToken);

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

            ItemValue taskValue = new()
            {
                ItemFieldVersionId = itemFieldVersion.Id,
                ItemVersionId = newItemVersionEntry.Entity.Id,
                ItemFieldVersion = itemFieldVersion
            };

            if (value is not null)
            {
                bool valid = taskValue.TrySetValue(value);
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

            dbContext.ItemValues.Add(taskValue);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass(nameof(ItemVersion), newItemVersionEntry.Entity.Id);
    }
}