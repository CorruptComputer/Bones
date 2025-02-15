using Bones.Database.DbSets.GenericItems;
using Bones.Database.DbSets.ProjectManagement;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Bones.Database.Operations.GenericItem;

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
            RuleFor(x => x.FriendlyIdPrefix).NotNull().NotEmpty().MaximumLength(6).Matches(@"^[a-zA-Z]*$");
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

        GenericItemLayout newLayout = new()
        {
            Project = project,
            FriendlyIdPrefix = request.FriendlyIdPrefix
        };

        EntityEntry<GenericItemLayout> added = await dbContext.ItemLayouts.AddAsync(newLayout, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass(added.Entity.Id);
    }
}