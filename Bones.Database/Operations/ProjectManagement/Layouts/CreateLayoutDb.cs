using Bones.Database.DbSets.ProjectManagement;
using Bones.Shared.Backend.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Bones.Database.Operations.ProjectManagement.Layouts;

// TODO: LayoutVersions
public sealed class CreateLayoutDb(BonesDbContext dbContext) : IRequestHandler<CreateLayoutDb.Command, CommandResponse>
{
    /// <summary>
    ///     DB Command for creating a layout.
    /// </summary>
    /// <param name="Name">Name of the layout</param>
    /// <param name="ItemFieldIds">Internal IDs of the fields this layout is using</param>
    public record Command(string Name, List<Guid> ItemFieldIds) : IValidatableRequest<CommandResponse>
    {
        /// <inheritdoc />
        public (bool valid, string? invalidReason) IsRequestValid()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                return (false, "");
            }

            foreach (Guid id in ItemFieldIds)
            {
                if (id == Guid.Empty)
                {
                    return (false, "");
                }
            }

            return (true, null);
        }
    }

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        List<ItemField> itemFields = [];

        foreach (Guid id in request.ItemFieldIds)
        {
            ItemField? itemField = await dbContext.ItemFields.FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
            if (itemField == null)
            {
                return new()
                {
                    Success = false,
                    FailureReason = "Invalid ItemFieldId."
                };
            }
            itemFields.Add(itemField);
        }

        ItemLayoutVersion initialVersion = new()
        {
            Version = 1,
            ItemLayout = new()
            {
                Name = request.Name
            },
            Fields = itemFields
        };

        EntityEntry<ItemLayoutVersion> created = await dbContext.ItemLayoutVersions.AddAsync(initialVersion, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return new()
        {
            Success = true,
            Id = created.Entity.Id
        };
    }
}