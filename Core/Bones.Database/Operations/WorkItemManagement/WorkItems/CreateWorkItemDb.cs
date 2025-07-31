using Bones.Database.DbSets.GenericItems;
using Bones.Database.DbSets.WorkItemManagement;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Bones.Database.Operations.WorkItemManagement.WorkItems;

/// <inheritdoc />
public sealed class CreateWorkItemDb(BonesDbContext dbContext) : IRequestHandler<CreateWorkItemDb.Command, CommandResponse>
{
    /// <summary>
    ///   DB Command for creating an WorkItem.
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
        WorkItemQueue? queue = await dbContext.WorkItemQueues.FirstOrDefaultAsync(q => q.Id == request.QueueId, cancellationToken);
        if (queue == null)
        {
            return CommandResponse.Fail("Invalid Queue ID.");
        }

        GenericItemLayout? itemLayout = await dbContext.ItemLayouts.Include(l => l.Project).FirstOrDefaultAsync(l => l.Id == request.ItemLayoutId, cancellationToken);
        if (itemLayout == null)
        {
            return CommandResponse.Fail("Invalid ItemLayout ID.");
        }

        EntityEntry<WorkItem> created = await dbContext.WorkItems.AddAsync(new()
        {
            WorkItemQueue = queue,
            AddedToQueueDateTime = request.ActionDateTime,
            Item = new()
            {
                FriendlyId = $"{itemLayout.FriendlyIdPrefix}-{itemLayout.FriendlyIdNonce++}",
                Project = itemLayout.Project,
                GenericItemLayout = itemLayout
            }
        }, cancellationToken);

        dbContext.ItemLayouts.Update(itemLayout);

        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass(nameof(WorkItem), created.Entity.Id);
    }
}