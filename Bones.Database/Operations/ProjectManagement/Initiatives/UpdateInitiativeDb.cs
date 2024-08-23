using Bones.Database.DbSets.ProjectManagement;
using Bones.Shared.Backend.Models;

namespace Bones.Database.Operations.ProjectManagement.Initiatives;

public sealed class UpdateInitiativeDb(BonesDbContext dbContext) : IRequestHandler<UpdateInitiativeDb.Command, CommandResponse>
{
    /// <summary>
    ///     DB Command for updating an Initiative.
    /// </summary>
    /// <param name="InitiativeId">Internal ID of the initiative</param>
    /// <param name="Name">The new name of the initiative</param>
    public record Command(Guid InitiativeId, string Name) : IValidatableRequest<CommandResponse>
    {
        /// <inheritdoc />
        public (bool valid, string? invalidReason) IsRequestValid()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                return (false, "");
            }

            if (InitiativeId == Guid.Empty)
            {
                return (false, "");
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

        initiative.Name = request.Name;

        await dbContext.SaveChangesAsync(cancellationToken);

        return new()
        {
            Success = true
        };
    }
}