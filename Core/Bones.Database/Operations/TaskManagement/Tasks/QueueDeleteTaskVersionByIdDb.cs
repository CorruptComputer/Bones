using Bones.Database.DbSets.Items;

namespace Bones.Database.Operations.TaskManagement.Tasks;

/// <inheritdoc />
public sealed class QueueDeleteTaskVersionByIdDb(BonesDbContext dbContext) : IRequestHandler<QueueDeleteTaskVersionByIdDb.Command, CommandResponse>
{
    /// <summary>
    ///   DB Command for deleting an ItemVersion.
    /// </summary>
    /// <param name="ItemVersionId">Internal ID of the item version</param>
    public record Command(Guid ItemVersionId) : IRequest<CommandResponse>;

    /// <inheritdoc />
    public class Validator : AbstractValidator<Command>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.ItemVersionId).NotNull().NotEqual(Guid.Empty);
        }
    }

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        ItemVersion? taskVersion = await dbContext.ItemVersions
            .Include(itemVersion => itemVersion.ItemValues)
            .FirstOrDefaultAsync(iv => iv.Id == request.ItemVersionId, cancellationToken);

        if (taskVersion == null)
        {
            return CommandResponse.Fail("Invalid ItemVersionId.");
        }

        foreach (ItemValue value in taskVersion.ItemValues)
        {
            value.DeleteFlag = true;
        }

        taskVersion.DeleteFlag = true;

        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass();
    }
}
