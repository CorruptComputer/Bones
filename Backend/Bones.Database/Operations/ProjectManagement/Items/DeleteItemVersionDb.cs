using Bones.Database.DbSets.ProjectManagement;
using Bones.Shared.Backend.Models;

namespace Bones.Database.Operations.ProjectManagement.Items;

public sealed class DeleteItemVersionDb(BonesDbContext dbContext) : IRequestHandler<DeleteItemVersionDb.Command, CommandResponse>
{
    /// <summary>
    ///     DB Command for deleting an ItemVersion.
    /// </summary>
    /// <param name="ItemVersionId">Internal ID of the item version</param>
    public record Command(Guid ItemVersionId) : IValidatableRequest<CommandResponse>
    {
        /// <inheritdoc />
        public (bool valid, string? invalidReason) IsRequestValid()
        {
            if (ItemVersionId == Guid.Empty)
            {
                return (false, "");
            }

            return (true, null);
        }
    }

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        WorkItemVersion? itemVersion = await dbContext.WorkItemVersions
            .Include(itemVersion => itemVersion.Values)
            .FirstOrDefaultAsync(p => p.Id == request.ItemVersionId, cancellationToken);

        if (itemVersion == null)
        {
            return new()
            {
                Success = false,
                FailureReason = "Invalid ItemVersionId."
            };
        }

        foreach (WorkItemValue value in itemVersion.Values)
        {
            value.DeleteFlag = true;
        }

        itemVersion.DeleteFlag = true;

        await dbContext.SaveChangesAsync(cancellationToken);

        return new()
        {
            Success = true
        };
    }
}