using Bones.Database.DbSets.ProjectManagement.Items;
using Bones.Database.DbSets.ProjectManagement.Layouts;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Bones.Database.Operations.ProjectManagement.Items;

public sealed class CreateItemVersionDb(BonesDbContext dbContext) : IRequestHandler<CreateItemVersionDb.Command, CommandResponse>
{
    /// <summary>
    ///     DB Command for creating an ItemVersion.
    /// </summary>
    /// <param name="ItemID">Internal ID of the item</param>
    /// <param name="LayoutVersionId">Internal ID of the layout version this item is using</param>
    /// <param name="Values">Internal ID of the queue</param>
    public record Command(Guid ItemID, Guid LayoutVersionId, Dictionary<string, object> Values)
        : IValidatableRequest<CommandResponse>
    {
        /// <inheritdoc />
        public (bool valid, string? invalidReason) IsRequestValid()
        {
            if (ItemID == Guid.Empty)
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
        Item? item = await dbContext.Items.Include(item => item.Versions).FirstOrDefaultAsync(i => i.Id == request.ItemID, cancellationToken);
        if (item == null)
        {
            return new()
            {
                Success = false,
                FailureReason = "Invalid ItemID."
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


        ItemVersion newVersion = new()
        {
            Version = item.Versions.Count,
            Item = item,
            Layout = layoutVersion,
            Values = values
        };

        EntityEntry<ItemVersion> created = await dbContext.ItemVersions.AddAsync(newVersion, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return new()
        {
            Success = true,
            Id = created.Entity.Id
        };
    }
}