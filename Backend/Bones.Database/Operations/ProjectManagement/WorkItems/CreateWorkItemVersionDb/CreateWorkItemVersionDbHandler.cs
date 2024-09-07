using Bones.Database.DbSets.ProjectManagement.WorkItemFields;
using Bones.Database.DbSets.ProjectManagement.WorkItemLayouts;
using Bones.Database.DbSets.ProjectManagement.WorkItems;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Bones.Database.Operations.ProjectManagement.WorkItems.CreateWorkItemVersionDb;

internal sealed class CreateWorkItemVersionDbHandler(BonesDbContext dbContext) : IRequestHandler<CreateWorkItemVersionDbCommand, CommandResponse>
{
    public async Task<CommandResponse> Handle(CreateWorkItemVersionDbCommand request, CancellationToken cancellationToken)
    {
        WorkItem? item = await dbContext.WorkItems.Include(item => item.Versions).FirstOrDefaultAsync(i => i.Id == request.WorkItemId, cancellationToken);
        if (item == null)
        {
            return CommandResponse.Fail("Invalid ItemID.");
        }

        WorkItemLayoutVersion? layoutVersion = await dbContext.WorkItemLayoutVersions.Include(layoutVersion => layoutVersion.Fields).FirstOrDefaultAsync(lv => lv.Id == request.WorkItemLayoutVersionId, cancellationToken);
        if (layoutVersion == null)
        {
            return CommandResponse.Fail("Invalid LayoutVersionId.");
        }

        if (request.Values.Count > layoutVersion.Fields.Count)
        {
            return CommandResponse.Fail("Invalid values provided.");
        }

        List<WorkItemValue> values = [];

        foreach ((string? key, object? value) in request.Values)
        {
            WorkItemField? field = layoutVersion.Fields.Find(f => f.Name == key);
            if (field == null)
            {
                return CommandResponse.Fail($"Invalid field name provided: {key}");
            }

            WorkItemValue workItemValue = new() { Field = field };
            bool valid = workItemValue.TrySetValue(value);
            if (!valid)
            {
                return CommandResponse.Fail($"Invalid value provided for '{Enum.GetName(field.Type)}' field '{key}': {value}");
            }

            values.Add(workItemValue);
        }


        WorkItemVersion newVersion = new()
        {
            Version = item.Versions.Count,
            WorkItem = item,
            WorkItemLayout = layoutVersion,
            Values = values
        };

        EntityEntry<WorkItemVersion> created = await dbContext.WorkItemVersions.AddAsync(newVersion, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass(created.Entity.Id);
    }
}