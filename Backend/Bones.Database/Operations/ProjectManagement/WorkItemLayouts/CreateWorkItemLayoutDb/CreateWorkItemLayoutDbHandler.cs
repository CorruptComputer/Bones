using Bones.Database.DbSets.ProjectManagement;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Bones.Database.Operations.ProjectManagement.WorkItemLayouts.CreateWorkItemLayoutDb;

// TODO: LayoutVersions
public sealed class CreateWorkItemLayoutDbHandler(BonesDbContext dbContext) : IRequestHandler<CreateWorkItemLayoutDbHandler.Command, CommandResponse>
{
    /// <summary>
    ///     DB Command for creating a layout.
    /// </summary>
    /// <param name="Name">Name of the layout</param>
    /// <param name="ItemFieldIds">Internal IDs of the fields this layout is using</param>
    public record Command(string Name, List<Guid> ItemFieldIds) : IRequest<CommandResponse>;

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        List<WorkItemField> itemFields = [];

        foreach (Guid id in request.ItemFieldIds)
        {
            WorkItemField? itemField = await dbContext.WorkItemFields.FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
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

        WorkItemLayoutVersion initialVersion = new()
        {
            Version = 1,
            WorkItemLayout = new()
            {
                Name = request.Name
            },
            Fields = itemFields
        };

        EntityEntry<WorkItemLayoutVersion> created = await dbContext.WorkItemLayoutVersions.AddAsync(initialVersion, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return new()
        {
            Success = true,
            Id = created.Entity.Id
        };
    }
}