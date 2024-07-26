using Bones.Database.DbSets.ProjectManagement.Initiatives;

namespace Bones.Database.Operations.ProjectManagement.Initiatives;

public sealed class DeleteInitiativeDb(BonesDbContext dbContext) : IRequestHandler<DeleteInitiativeDb.Command, CommandResponse>
{
    /// <summary>
    ///     DB Command for deleting an Initiative.
    /// </summary>
    /// <param name="InitiativeId">Internal ID of the initiative</param>
    public record Command(Guid InitiativeId) : IValidatableRequest<CommandResponse>
    {
        /// <inheritdoc />
        public bool IsRequestValid()
        {
            if (InitiativeId == Guid.Empty)
            {
                return false;
            }

            return true;
        }
    }

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        // TODO: fix this
        IQueryable<Initiative> initiative = dbContext.Initiatives.Where(i => i.Id == request.InitiativeId);
        if (await initiative.AnyAsync(cancellationToken))
        {
            return new()
            {
                Success = false,
                FailureReason = "Invalid InitiativeId."
            };
        }

        await initiative.ExecuteDeleteAsync(cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new()
        {
            Success = true
        };
    }
}