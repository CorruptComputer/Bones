using Bones.Database.DbSets.GenericItems.ItemFields;
using Bones.Database.DbSets.GenericItems.ItemLayouts;
using Bones.Database.DbSets.GenericItems.Items;
using Bones.Database.DbSets.WorkItemManagement;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Bones.Database.Operations.WorkItemManagement.WorkItems.CreateWorkItemVersionDb;

internal sealed class CreateWorkItemVersionDbHandler(BonesDbContext dbContext) : IRequestHandler<CreateWorkItemVersionDbCommand, CommandResponse>
{
    public async Task<CommandResponse> Handle(CreateWorkItemVersionDbCommand request, CancellationToken cancellationToken)
    {
        WorkItem? workItem = await dbContext.WorkItems.Include(item => item.Item).FirstOrDefaultAsync(i => i.Id == request.WorkItemId, cancellationToken);
        if (workItem == null)
        {
            return CommandResponse.Fail("Invalid WorkItem ID.");
        }

        ItemLayoutVersion? layoutVersion = await dbContext.ItemLayoutVersions.Include(layoutVersion => layoutVersion.Fields).FirstOrDefaultAsync(lv => lv.Id == request.WorkItemLayoutVersionId, cancellationToken);
        if (layoutVersion == null)
        {
            return CommandResponse.Fail("Invalid LayoutVersionId.");
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

        workItem.Item.Versions.Add(new()
        {
            ItemId = workItem.Item.Id,
            Version = ++workItem.Item.CurrentVersion,
            CreateDateTime = DateTimeOffset.Now,
            ItemLayoutVersion = layoutVersion,
            Values = values
        });

        EntityEntry<WorkItem> updated = dbContext.WorkItems.Update(workItem);

        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass(updated.Entity.Item.Versions[updated.Entity.Item.CurrentVersion].Id);
    }
}