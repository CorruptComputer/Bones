using Bones.Database.DbSets.GenericItems.ItemFields;
using Bones.Database.DbSets.GenericItems.ItemLayouts;
using Bones.Database.DbSets.GenericItems.Items;
using Bones.Database.DbSets.WorkItemManagement;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Bones.Database.Operations.WorkItemManagement.WorkItems.CreateWorkItemDb;

internal sealed class CreateWorkItemDbHandler(BonesDbContext dbContext) : IRequestHandler<CreateWorkItemDbCommand, CommandResponse>
{
    public async Task<CommandResponse> Handle(CreateWorkItemDbCommand request, CancellationToken cancellationToken)
    {
        WorkItemQueue? queue = await dbContext.WorkItemQueues.FirstOrDefaultAsync(q => q.Id == request.QueueId, cancellationToken);
        if (queue == null)
        {
            return CommandResponse.Fail("Invalid QueueId.");
        }

        ItemLayoutVersion? layoutVersion = await dbContext.ItemLayoutVersions
            .Include(itemLayoutVersion => itemLayoutVersion.Fields)
            .FirstOrDefaultAsync(lv => lv.Id == request.WorkItemLayoutVersionId, cancellationToken);

        if (layoutVersion == null)
        {
            return CommandResponse.Fail("Invalid LayoutVersionId.");
        }

        ItemLayout? itemLayout = await dbContext.ItemLayouts.FindAsync([layoutVersion.ItemLayoutId], cancellationToken);
        if (itemLayout == null)
        {
            return CommandResponse.Fail("Could not find item layout, this has a high chance of being caused by a bug.");
        }

        if (request.Values.Count > layoutVersion.Fields.Count)
        {
            return CommandResponse.Fail("Invalid values provided.");
        }

        List<ItemValue> values = [];

        foreach ((string? key, object? value) in request.Values)
        {
            ItemField? field = layoutVersion.Fields.Find(f => f.Name == key);
            if (field == null)
            {
                return CommandResponse.Fail($"Invalid field name provided: {key}");
            }

            ItemValue workItemValue = new() { Field = field };
            bool valid = workItemValue.TrySetValue(value);
            if (!valid)
            {
                return CommandResponse.Fail($"Invalid value provided for '{Enum.GetName(field.Type)}' field '{key}': {value}");
            }

            values.Add(workItemValue);
        }

        WorkItem initialVersion = new()
        {
            WorkItemQueue = queue,
            AddedToQueueDateTime = DateTimeOffset.UtcNow,
            Item = new()
            {
                Name = request.Name,
                ProjectId = itemLayout.ProjectId,
                ItemLayout = itemLayout
            }
        };

        EntityEntry<WorkItem> created = await dbContext.WorkItems.AddAsync(initialVersion, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass(created.Entity.Id);
    }
}