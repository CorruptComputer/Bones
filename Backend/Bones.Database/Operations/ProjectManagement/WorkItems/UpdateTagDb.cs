using Bones.Database.DbSets.ProjectManagement;

namespace Bones.Database.Operations.ProjectManagement.WorkItems;

public sealed class UpdateTagDb(BonesDbContext dbContext) : IRequestHandler<UpdateTagDb.Command, CommandResponse>
{
    /// <summary>
    ///     DB Command for updating a Tag.
    /// </summary>
    /// <param name="TagId">Internal ID of the tag</param>
    /// <param name="Name">The new name of the tag</param>
    public record Command(Guid TagId, string Name) : IValidatableRequest<CommandResponse>
    {
        /// <inheritdoc />
        public (bool valid, string? invalidReason) IsRequestValid()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                return (false, "");
            }

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
        Tag? tag = await dbContext.Tags.FirstOrDefaultAsync(t => t.Id == request.TagId, cancellationToken);
        if (tag == null)
        {
            return new()
            {
                Success = false,
                FailureReason = "Invalid TagId."
            };
        }

        tag.Name = request.Name;

        await dbContext.SaveChangesAsync(cancellationToken);

        return new()
        {
            Success = true
        };
    }
}