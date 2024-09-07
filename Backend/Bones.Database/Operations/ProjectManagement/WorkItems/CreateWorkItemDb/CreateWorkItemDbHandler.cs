using Bones.Database.DbSets.ProjectManagement;
using Bones.Database.DbSets.ProjectManagement.WorkItemFields;
using Bones.Database.DbSets.ProjectManagement.WorkItemLayouts;
using Bones.Database.DbSets.ProjectManagement.WorkItems;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Bones.Database.Operations.ProjectManagement.WorkItems.CreateWorkItemDb;

internal sealed class CreateWorkItemDbHandler(BonesDbContext dbContext) : IRequestHandler<CreateWorkItemDbCommand, CommandResponse>
{
    public async Task<CommandResponse> Handle(CreateWorkItemDbCommand request, CancellationToken cancellationToken)
    {
        Queue? queue = await dbContext.Queues.FirstOrDefaultAsync(q => q.Id == request.QueueId, cancellationToken);
        if (queue == null)
        {
            return CommandResponse.Fail("Invalid QueueId.");
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


        WorkItemVersion initialVersion = new()
        {
            Version = 0,
            WorkItem = new()
            {
                Name = request.Name,
                Queue = queue,
                AddedToQueueDateTime = DateTimeOffset.UtcNow
            },
            WorkItemLayout = layoutVersion,
            Values = values
        };

        EntityEntry<WorkItemVersion> created = await dbContext.WorkItemVersions.AddAsync(initialVersion, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass(created.Entity.Id);
    }
}