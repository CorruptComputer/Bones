using Bones.Database.DbSets.ProjectManagement.Items;
using Bones.Database.DbSets.ProjectManagement.Queues;

namespace Bones.Database.Operations.ProjectManagement.Items;

public sealed class UpdateItemDb(BonesDbContext dbContext) : IRequestHandler<UpdateItemDb.Command, CommandResponse>
{
    /// <summary>
    ///     DB Command for updating an Item.
    /// </summary>
    /// <param name="ItemId">Internal ID of the item</param>
    /// <param name="Name">The new name of the item</param>
    public record Command(Guid ItemId, string Name, Guid QueueId, List<Guid> TagIds) : IValidatableRequest<CommandResponse>
    {
        /// <inheritdoc />
        public bool IsRequestValid()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                return false;
            }

            if (ItemId == Guid.Empty)
            {
                return false;
            }

            if (QueueId == Guid.Empty)
            {
                return false;
            }

            if (TagIds.Exists(tagId => tagId == Guid.Empty))
            {
                return false;
            }

            return true;
        }
    }

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        Item? item = await dbContext.Items.FirstOrDefaultAsync(p => p.Id == request.ItemId, cancellationToken);
        if (item == null)
        {
            return new()
            {
                Success = false,
                FailureReason = "Invalid ItemId."
            };
        }

        Queue? queue = await dbContext.Queues.FirstOrDefaultAsync(q => q.Id == request.QueueId, cancellationToken);
        if (queue == null)
        {
            return new()
            {
                Success = false,
                FailureReason = "Invalid QueueId."
            };
        }

        List<Tag> tags = [];
        foreach (Guid tagId in request.TagIds)
        {
            Tag? tag = await dbContext.Tags.FirstOrDefaultAsync(t => t.Id == tagId, cancellationToken);
            if (tag == null)
            {
                return new()
                {
                    Success = false,
                    FailureReason = $"Invalid TagId: {tagId}"
                };
            }
            tags.Add(tag);
        }

        item.Name = request.Name;
        item.Queue = queue;
        item.AddedToQueueDateTime = DateTimeOffset.Now;
        item.Tags = tags;

        await dbContext.SaveChangesAsync(cancellationToken);

        return new()
        {
            Success = true
        };
    }
}