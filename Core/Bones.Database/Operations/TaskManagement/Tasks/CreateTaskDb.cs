using Bones.Database.DbSets.Items;
using Bones.Database.DbSets.TaskManagement;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Bones.Database.Operations.TaskManagement.Tasks;

/// <inheritdoc />
public sealed class CreateTaskDb(BonesDbContext dbContext) : IRequestHandler<CreateTaskDb.Command, CommandResponse>
{
    /// <summary>
    ///   DB Command for creating an Task.
    /// </summary>
    /// <param name="QueueId">Internal ID of the queue this item is in</param>
    /// <param name="ItemLayoutId">Internal ID of the layout this item is using</param>
    /// <param name="ActionDateTime"></param>
    public sealed record Command(Guid QueueId, Guid ItemLayoutId, DateTimeOffset ActionDateTime) : IRequest<CommandResponse>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Command>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.QueueId).NotNull().NotEqual(Guid.Empty);
            RuleFor(x => x.ItemLayoutId).NotNull().NotEqual(Guid.Empty);
        }
    }

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        TaskQueue? queue = await dbContext.TaskQueues.FirstOrDefaultAsync(q => q.Id == request.QueueId, cancellationToken);
        if (queue == null)
        {
            return CommandResponse.Fail("Invalid Queue ID.");
        }

        ItemLayout? itemLayout = await dbContext.ItemLayouts.Include(l => l.Project).FirstOrDefaultAsync(l => l.Id == request.ItemLayoutId, cancellationToken);
        if (itemLayout == null)
        {
            return CommandResponse.Fail("Invalid ItemLayout ID.");
        }

        EntityEntry<Item> item = await dbContext.Items.AddAsync(new()
        {
            FriendlyId = $"{itemLayout.FriendlyIdPrefix}-{itemLayout.FriendlyIdNonce++}",
            ProjectId = itemLayout.ProjectId,
            ItemLayoutId = itemLayout.Id
        }, cancellationToken);

        dbContext.ItemLayouts.Update(itemLayout);

        await dbContext.SaveChangesAsync(cancellationToken);

        EntityEntry<BonesTask> created = await dbContext.Tasks.AddAsync(new()
        {
            TaskQueueId = queue.Id,
            AddedToQueueDateTime = request.ActionDateTime,
            ItemId = item.Entity.Id
        }, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass(nameof(BonesTask), created.Entity.Id);
    }
}