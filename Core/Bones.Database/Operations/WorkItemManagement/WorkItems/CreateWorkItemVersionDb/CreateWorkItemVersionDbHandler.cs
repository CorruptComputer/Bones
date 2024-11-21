using Bones.Database.DbSets.GenericItems.GenericItemFields;
using Bones.Database.DbSets.GenericItems.GenericItemLayouts;
using Bones.Database.DbSets.GenericItems.GenericItems;
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

        GenericItemLayoutVersion? layoutVersion = await dbContext.ItemLayoutVersions.Include(layoutVersion => layoutVersion.Fields).FirstOrDefaultAsync(lv => lv.Id == request.WorkItemLayoutVersionId, cancellationToken);
        if (layoutVersion == null)
        {
            return CommandResponse.Fail("Invalid LayoutVersionId.");
        }

        if (request.Values.Count > layoutVersion.Fields.Count)
        {
            return CommandResponse.Fail("Invalid values provided.");
        }

        List<GenericItemValue> values = [];

        foreach ((string? key, object? value) in request.Values)
        {
            GenericItemField? field = layoutVersion.Fields.Find(f => f.Name == key);
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
            ItemId = workItem.Item.Id,
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