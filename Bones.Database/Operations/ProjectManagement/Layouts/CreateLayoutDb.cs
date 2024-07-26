using Bones.Database.DbSets.ProjectManagement.Items;
using Bones.Database.DbSets.ProjectManagement.Layouts;
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
        public bool IsRequestValid()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                return false;
            }

            foreach (Guid id in ItemFieldIds)
            {
                if (id == Guid.Empty)
                {
                    return false;
                }
            }

            return true;
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

        LayoutVersion initialVersion = new()
        {
            Version = 1,
            Layout = new()
            {
                Name = request.Name
            },
            Fields = itemFields
        };

        EntityEntry<LayoutVersion> created = await dbContext.LayoutVersions.AddAsync(initialVersion, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return new()
        {
            Success = true,
            Id = created.Entity.Id
        };
    }
}