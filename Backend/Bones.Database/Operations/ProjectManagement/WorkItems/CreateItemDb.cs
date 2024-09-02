using Bones.Database.DbSets.ProjectManagement;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Bones.Database.Operations.ProjectManagement.WorkItems;

public sealed class CreateItemDb(BonesDbContext dbContext) : IRequestHandler<CreateItemDb.Command, CommandResponse>
{
    /// <summary>
    ///     DB Command for creating an WorkItem.
    /// </summary>
    /// <param name="Name">Name of the item</param>
    /// <param name="QueueId">Internal ID of the queue this item is in</param>
    /// <param name="LayoutVersionId">Internal ID of the layout version this item is using</param>
    /// <param name="Values">Internal ID of the queue</param>
    public record Command(string Name, Guid QueueId, Guid LayoutVersionId, Dictionary<string, object> Values)
        : IRequest<CommandResponse>;

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        Queue? queue = await dbContext.Queues.FirstOrDefaultAsync(q => q.Id == request.QueueId, cancellationToken);
        if (queue == null)
        {
            return CommandResponse.Fail("Invalid QueueId.");
        }

        WorkItemLayoutVersion? layoutVersion = await dbContext.WorkItemLayoutVersions.Include(layoutVersion => layoutVersion.Fields).FirstOrDefaultAsync(lv => lv.Id == request.LayoutVersionId, cancellationToken);
        if (layoutVersion == null)
        {
            return new()
            {
                Success = false,
                FailureReason = "Invalid LayoutVersionId."
            };
        }

        if (request.Values.Count > layoutVersion.Fields.Count)
        {
            return new()
            {
                Success = false,
                FailureReason = "Invalid values provided."
            };
        }

        List<WorkItemValue> values = [];

        foreach ((string? key, object? value) in request.Values)
        {
            WorkItemField? field = layoutVersion.Fields.Find(f => f.Name == key);
            if (field == null)
            {
                return new()
                {
                    Success = false,
                    FailureReason = $"Invalid field name provided: {key}"
                };
            }

            WorkItemValue workItemValue = new() { Field = field };
            bool valid = workItemValue.TrySetValue(value);
            if (!valid)
            {
                return new()
                {
                    Success = false,
                    FailureReason = $"Invalid value provided for '{Enum.GetName(field.Type)}' field '{key}': {value}"
                };
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

        return new()
        {
            Success = true,
            Id = created.Entity.Id
        };
    }
}