using Bones.Database.DbSets.GenericItems;
using Bones.Database.DbSets.WorkItemManagement;

namespace Bones.Database.Operations.WorkItemManagement.WorkItems;

/// <inheritdoc />
public sealed class QueueDeleteWorkItemByIdDb(BonesDbContext dbContext, ISender sender) : IRequestHandler<QueueDeleteWorkItemByIdDb.Command, CommandResponse>
{
    /// <summary>
    ///   DB Command for deleting an WorkItem.
    /// </summary>
    /// <param name="WorkItemId">Internal ID of the item</param>
    public record Command(Guid WorkItemId) : IRequest<CommandResponse>;

    /// <inheritdoc />
    public class Validator : AbstractValidator<Command>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.WorkItemId).NotNull().NotEqual(Guid.Empty);
        }
    }

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        WorkItem? workItem = await dbContext.WorkItems
            .Include(item => item.Item)
            .ThenInclude(item => item.Versions)
            .FirstOrDefaultAsync(p => p.Id == request.WorkItemId, cancellationToken);

        if (workItem == null)
        {
            return CommandResponse.Fail("Invalid ItemId.");
        }

        foreach (GenericItemVersion version in workItem.Item.Versions)
        {
            await sender.Send(new QueueDeleteWorkItemVersionByIdDb.Command(version.Id), cancellationToken);
        }

        workItem.DeleteFlag = true;

        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass();
    }
}