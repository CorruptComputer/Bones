using Bones.Database.DbSets.ProjectManagement;
using Bones.Database.DbSets.TaskManagement;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Bones.Database.Operations.TaskManagement.TaskQueues;

/// <inheritdoc />
public sealed class CreateTaskQueueDb(BonesDbContext dbContext) : IRequestHandler<CreateTaskQueueDb.Command, CommandResponse>
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
            RuleFor(x => x.Name).NotEmpty();
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

        EntityEntry<TaskQueue> created = await dbContext.TaskQueues.AddAsync(new()
        {
            Name = request.Name,
            InitiativeId = initiative.Id
        }, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass(nameof(TaskQueue), created.Entity.Id);
    }
}
