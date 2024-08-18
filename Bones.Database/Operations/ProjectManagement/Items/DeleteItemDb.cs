using Bones.Database.DbSets.ProjectManagement.Items;

namespace Bones.Database.Operations.ProjectManagement.Items;

public sealed class DeleteItemDb(BonesDbContext dbContext, ISender sender) : IRequestHandler<DeleteItemDb.Command, CommandResponse>
{
    /// <summary>
    ///     DB Command for deleting an Item.
    /// </summary>
    /// <param name="ItemId">Internal ID of the item</param>
    public record Command(Guid ItemId) : IValidatableRequest<CommandResponse>
    {
        /// <inheritdoc />
        public (bool valid, string? invalidReason) IsRequestValid()
        {
            if (ItemId == Guid.Empty)
            {
                return (false, "");
            }

            return (true, null);
        }
    }

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        Item? item = await dbContext.Items
            .Include(item => item.Versions)
            .ThenInclude(itemVersion => itemVersion.Values)
            .FirstOrDefaultAsync(p => p.Id == request.ItemId, cancellationToken);

        if (item == null)
        {
            return new()
            {
                Success = false,
                FailureReason = "Invalid ItemId."
            };
        }

        foreach (ItemVersion version in item.Versions)
        {
            await sender.Send(new DeleteItemVersionDb.Command(version.Id), cancellationToken);
        }

        item.DeleteFlag = true;

        await dbContext.SaveChangesAsync(cancellationToken);

        return new()
        {
            Success = true
        };
    }
}