using Bones.Database.DbSets.ProjectManagement;
using Bones.Shared.Backend.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Bones.Database.Operations.ProjectManagement.Items;

public sealed class CreateItemVersionDb(BonesDbContext dbContext) : IRequestHandler<CreateItemVersionDb.Command, CommandResponse>
{
    /// <summary>
    ///     DB Command for creating an ItemVersion.
    /// </summary>
    /// <param name="ItemId">Internal ID of the item</param>
    /// <param name="LayoutVersionId">Internal ID of the layout version this item is using</param>
    /// <param name="Values">Internal ID of the queue</param>
    public record Command(Guid ItemId, Guid LayoutVersionId, Dictionary<string, object> Values)
        : IValidatableRequest<CommandResponse>
    {
        /// <inheritdoc />
        public (bool valid, string? invalidReason) IsRequestValid()
        {
            if (ItemId == Guid.Empty)
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
        WorkItem? item = await dbContext.WorkItems.Include(item => item.Versions).FirstOrDefaultAsync(i => i.Id == request.ItemId, cancellationToken);
        if (item == null)
        {
            return new()
            {
                Success = false,
                FailureReason = "Invalid ItemID."
            };
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


        WorkItemVersion newVersion = new()
        {
            Version = item.Versions.Count,
            WorkItem = item,
            WorkItemLayout = layoutVersion,
            Values = values
        };

        EntityEntry<WorkItemVersion> created = await dbContext.WorkItemVersions.AddAsync(newVersion, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return new()
        {
            Success = true,
            Id = created.Entity.Id
        };
    }
}