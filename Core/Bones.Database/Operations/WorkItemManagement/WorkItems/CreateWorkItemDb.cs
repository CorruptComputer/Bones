using Bones.Database.DbSets.GenericItems;
using Bones.Database.DbSets.WorkItemManagement;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Bones.Database.Operations.WorkItemManagement.WorkItems;

/// <inheritdoc />
public sealed class CreateWorkItemDb(BonesDbContext dbContext) : IRequestHandler<CreateWorkItemDb.Command, CommandResponse>
{
    /// <summary>
    ///     DB Command for creating an WorkItem.
    /// </summary>
    /// <param name="Name">Name of the item</param>
    /// <param name="QueueId">Internal ID of the queue this item is in</param>
    /// <param name="ItemLayoutId">Internal ID of the layout this item is using</param>
    public sealed record Command(string Name, Guid QueueId, Guid ItemLayoutId) : IRequest<CommandResponse>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Command>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.Name).NotNull().NotEmpty();
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
            AddedToQueueDateTime = DateTimeOffset.UtcNow,
            Item = new()
            {
                Name = request.Name,
                Project = itemLayout.Project,
                GenericItemLayout = itemLayout
            }
        }, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass(created.Entity.Id);
    }
}