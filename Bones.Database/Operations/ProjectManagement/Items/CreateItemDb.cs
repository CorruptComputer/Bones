using Bones.Database.DbSets.ProjectManagement.Items;
using Bones.Database.DbSets.ProjectManagement.Layouts;
using Bones.Database.DbSets.ProjectManagement.Queues;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Bones.Database.Operations.ProjectManagement.Items;

public sealed class CreateItemDb(BonesDbContext dbContext) : IRequestHandler<CreateItemDb.Command, CommandResponse>
{
    /// <summary>
    ///     DB Command for creating an Item.
    /// </summary>
    /// <param name="Name">Name of the item</param>
    /// <param name="QueueId">Internal ID of the queue this item is in</param>
    /// <param name="LayoutVersionId">Internal ID of the layout version this item is using</param>
    /// <param name="Values">Internal ID of the queue</param>
    public record Command(string Name, Guid QueueId, Guid LayoutVersionId, Dictionary<string, object> Values)
        : IValidatableRequest<CommandResponse>
    {
        /// <inheritdoc />
        public (bool valid, string? invalidReason) IsRequestValid()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                return (false, "");
            }

            if (QueueId == Guid.Empty)
            {
                return (false, "");
            }

            if (LayoutVersionId == Guid.Empty)
            {
                return (false, "");
            }

            return (true, null);
        }
    }

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        Queue? queue = await dbContext.Queues.FirstOrDefaultAsync(q => q.Id == request.QueueId, cancellationToken);
        if (queue == null)
        {
            return new()
            {
                Success = false,
                FailureReason = "Invalid QueueId."
            };
        }

        LayoutVersion? layoutVersion = await dbContext.LayoutVersions.Include(layoutVersion => layoutVersion.Fields).FirstOrDefaultAsync(lv => lv.Id == request.LayoutVersionId, cancellationToken);
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

        List<ItemValue> values = [];

        foreach ((string? key, object? value) in request.Values)
        {
            ItemField? field = layoutVersion.Fields.Find(f => f.Name == key);
            if (field == null)
            {
                return new()
                {
                    Success = false,
                    FailureReason = $"Invalid field name provided: {key}"
                };
            }

            ItemValue itemValue = new() { Field = field };
            bool valid = itemValue.TrySetValue(value);
            if (!valid)
            {
                return new()
                {
                    Success = false,
                    FailureReason = $"Invalid value provided for '{Enum.GetName(field.Type)}' field '{key}': {value}"
                };
            }

            values.Add(itemValue);
        }


        ItemVersion initialVersion = new()
        {
            Version = 0,
            Item = new()
            {
                Name = request.Name,
                Queue = queue,
                AddedToQueueDateTime = DateTimeOffset.UtcNow
            },
            Layout = layoutVersion,
            Values = values
        };

        EntityEntry<ItemVersion> created = await dbContext.ItemVersions.AddAsync(initialVersion, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return new()
        {
            Success = true,
            Id = created.Entity.Id
        };
    }
}