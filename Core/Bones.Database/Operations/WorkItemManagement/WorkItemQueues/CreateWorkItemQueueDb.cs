using Bones.Database.DbSets.ProjectManagement;
using Bones.Database.DbSets.WorkItemManagement;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Bones.Database.Operations.WorkItemManagement.WorkItemQueues;

/// <inheritdoc />
public sealed class CreateWorkItemQueueDb(BonesDbContext dbContext) : IRequestHandler<CreateWorkItemQueueDb.Command, CommandResponse>
{
    /// <summary>
    ///   DB Command for creating a Queue.
    /// </summary>
    /// <param name="Name">Name of the queue</param>
    /// <param name="InitiativeId">Internal ID of the initiative</param>
    public sealed record Command(string Name, Guid InitiativeId) : IRequest<CommandResponse>;

    /// <inheritdoc />
    public class Validator : AbstractValidator<Command>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.Name).NotNull().NotEmpty();
            RuleFor(x => x.InitiativeId).NotNull().NotEqual(Guid.Empty);
        }
    }

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        Initiative? initiative = await dbContext.Initiatives.FirstOrDefaultAsync(i => i.Id == request.InitiativeId, cancellationToken);
        if (initiative == null)
        {
            return CommandResponse.Fail("Invalid InitiativeId.");
        }

        EntityEntry<WorkItemQueue> created = await dbContext.WorkItemQueues.AddAsync(new()
        {
            Name = request.Name,
            Initiative = initiative
        }, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass(nameof(WorkItemQueue), created.Entity.Id);
    }
}
