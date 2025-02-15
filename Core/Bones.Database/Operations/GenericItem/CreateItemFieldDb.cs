using Bones.Database.DbSets.GenericItems;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Bones.Database.Operations.GenericItem;


/// <inheritdoc />
public sealed class CreateItemFieldDb(BonesDbContext dbContext) : IRequestHandler<CreateItemFieldDb.Command, CommandResponse>
{
    /// <summary>
    ///     DB Command for creating a new item field
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
        EntityEntry<GenericItemField> added = dbContext.ItemFields.Add(new()
        {
            ProjectId = request.ProjectId,
        });

        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass(added.Entity.Id);
    }
}
