using Bones.Database.DbSets.Items.Layouts;
using Bones.Database.DbSets.Projects;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Bones.Database.Operations.Items.Layouts;

/// <inheritdoc />
public class CreateItemLayoutDb(BonesDbContext dbContext) : IRequestHandler<CreateItemLayoutDb.Command, CommandResponse>
{
    /// <summary>
    ///   DB Command for creating an Item Layout
    /// </summary>
    /// <param name="FriendlyIdPrefix"></param>
    /// <param name="ProjectId"></param>
    public sealed record Command(Guid ProjectId, string FriendlyIdPrefix) : IRequest<CommandResponse>;

    /// <inheritdoc />
    public class Validator : AbstractValidator<Command>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.ProjectId).NotNull().NotEqual(Guid.Empty);
            RuleFor(x => x.FriendlyIdPrefix).NotEmpty().MaximumLength(6).Matches(@"^[a-zA-Z]*$");
        }
    }

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {

        Project? project = await dbContext.Projects.FindAsync([request.ProjectId], cancellationToken);

        if (project == null)
        {
            return CommandResponse.Fail("Project does not exist");
        }

        ItemLayout newLayout = new()
        {
            ProjectId = project.Id,
            FriendlyIdPrefix = request.FriendlyIdPrefix
        };

        EntityEntry<ItemLayout> added = await dbContext.ItemLayouts.AddAsync(newLayout, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass(nameof(ItemLayout), added.Entity.Id);
    }
}