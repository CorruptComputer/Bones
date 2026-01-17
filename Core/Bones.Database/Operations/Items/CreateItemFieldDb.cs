using Bones.Database.DbSets.Items;
using Bones.Database.DbSets.ProjectManagement;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Bones.Database.Operations.Items;


/// <inheritdoc />
public sealed class CreateItemFieldDb(BonesDbContext dbContext) : IRequestHandler<CreateItemFieldDb.Command, CommandResponse>
{
    /// <summary>
    ///   DB Command for creating a new item field
    /// </summary>
    /// <param name="ProjectId">Internal ID of the project</param>
    public record Command(Guid ProjectId) : IRequest<CommandResponse>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Command>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.ProjectId).NotNull().NotEqual(Guid.Empty);
        }
    }

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        Project? project = await dbContext.Projects.FindAsync([request.ProjectId], cancellationToken);

        if (project == null || project.DeleteFlag)
        {
            return CommandResponse.Fail("Project not found");
        }

        EntityEntry<ItemField> added = dbContext.ItemFields.Add(new()
        {
            ProjectId = project.Id,
        });

        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass(nameof(ItemField), added.Entity.Id);
    }
}
