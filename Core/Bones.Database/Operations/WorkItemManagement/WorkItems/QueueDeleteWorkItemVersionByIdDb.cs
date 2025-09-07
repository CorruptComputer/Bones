using Bones.Database.DbSets.GenericItems;

namespace Bones.Database.Operations.WorkItemManagement.WorkItems;

/// <inheritdoc />
public sealed class QueueDeleteWorkItemVersionByIdDb(BonesDbContext dbContext) : IRequestHandler<QueueDeleteWorkItemVersionByIdDb.Command, CommandResponse>
{
    /// <summary>
    ///   DB Command for deleting an ItemVersion.
    /// </summary>
    /// <param name="WorkItemVersionId">Internal ID of the item version</param>
    public record Command(Guid WorkItemVersionId) : IRequest<CommandResponse>;

    /// <inheritdoc />
    public class Validator : AbstractValidator<Command>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.WorkItemVersionId).NotNull().NotEqual(Guid.Empty);
        }
    }

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        GenericItemVersion? workItemVersion = await dbContext.ItemVersions
            .Include(itemVersion => itemVersion.Values)
            .FirstOrDefaultAsync(p => p.Id == request.WorkItemVersionId, cancellationToken);

        if (workItemVersion == null)
        {
            return CommandResponse.Fail("Invalid ItemVersionId.");
        }

        foreach (GenericItemValue value in workItemVersion.Values)
        {
            value.DeleteFlag = true;
        }

        workItemVersion.DeleteFlag = true;

        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass();
    }
}
