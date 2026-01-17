using Bones.Database.DbSets.Items;
using Bones.Database.DbSets.TaskManagement;

namespace Bones.Database.Operations.TaskManagement.Tasks;

/// <inheritdoc />
public sealed class QueueDeleteTaskByIdDb(BonesDbContext dbContext, ISender sender) : IRequestHandler<QueueDeleteTaskByIdDb.Command, CommandResponse>
{
    /// <summary>
    ///   DB Command for deleting an Task.
    /// </summary>
    /// <param name="TaskId">Internal ID of the item</param>
    public record Command(Guid TaskId) : IRequest<CommandResponse>;

    /// <inheritdoc />
    public class Validator : AbstractValidator<Command>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.TaskId).NotNull().NotEqual(Guid.Empty);
        }
    }

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        BonesTask? task = await dbContext.Tasks
            .Include(item => item.Item)
                .ThenInclude(item => item!.Versions)
            .FirstOrDefaultAsync(p => p.Id == request.TaskId, cancellationToken);

        if (task is null || task.Item is null)
        {
            return CommandResponse.Fail("Invalid ItemId.");
        }

        foreach (ItemVersion version in task.Item.Versions)
        {
            await sender.Send(new QueueDeleteTaskVersionByIdDb.Command(version.Id), cancellationToken);
        }

        task.DeleteFlag = true;

        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass();
    }
}