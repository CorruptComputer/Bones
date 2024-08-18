using Bones.Database.DbSets.ProjectManagement.Items;

namespace Bones.Database.Operations.ProjectManagement.Items;

public sealed class DeleteTagDb(BonesDbContext dbContext) : IRequestHandler<DeleteTagDb.Command, CommandResponse>
{
    /// <summary>
    ///     DB Command for deleting a Tag.
    /// </summary>
    /// <param name="TagId">Internal ID of the tag</param>
    public record Command(Guid TagId) : IValidatableRequest<CommandResponse>
    {
        /// <inheritdoc />
        public (bool valid, string? invalidReason) IsRequestValid()
        {
            if (TagId == Guid.Empty)
            {
                return (false, "");
            }

            return (true, null);
        }
    }

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        IQueryable<Tag> tag = dbContext.Tags.Where(p => p.Id == request.TagId);
        if (await tag.AnyAsync(cancellationToken))
        {
            return new()
            {
                Success = false,
                FailureReason = "Invalid TagId."
            };
        }

        await tag.ExecuteDeleteAsync(cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new()
        {
            Success = true
        };
    }
}