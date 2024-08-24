using Bones.Database.DbSets.ProjectManagement;
using Bones.Shared.Backend.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Bones.Database.Operations.ProjectManagement.Queues;

public sealed class CreateQueueDb(BonesDbContext dbContext) : IRequestHandler<CreateQueueDb.Command, CommandResponse>
{
    /// <summary>
    ///     DB Command for creating a Queue.
    /// </summary>
    /// <param name="Name">Name of the queue</param>
    /// <param name="InitiativeId">Internal ID of the initiative</param>
    public record Command(string Name, Guid InitiativeId) : IValidatableRequest<CommandResponse>
    {
        /// <inheritdoc />
        public (bool valid, string? invalidReason) IsRequestValid()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                return (false, "Name is required.");
            }

            if (InitiativeId == Guid.Empty)
            {
                return (false, "Initiative id is required.");
            }

            return (true, null);
        }
    }

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        Initiative? initiative = await dbContext.Initiatives.FirstOrDefaultAsync(i => i.Id == request.InitiativeId, cancellationToken);
        if (initiative == null)
        {
            return new()
            {
                Success = false,
                FailureReason = "Invalid InitiativeId."
            };
        }

        EntityEntry<Queue> created = await dbContext.Queues.AddAsync(new()
        {
            Name = request.Name,
            Initiative = initiative
        }, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return new()
        {
            Success = true,
            Id = created.Entity.Id
        };
    }
}