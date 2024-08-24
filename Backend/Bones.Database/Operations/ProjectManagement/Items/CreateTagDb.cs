using Bones.Database.DbSets.ProjectManagement;
using Bones.Shared.Backend.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Bones.Database.Operations.ProjectManagement.Items;

public sealed class CreateTagDb(BonesDbContext dbContext) : IRequestHandler<CreateTagDb.Command, CommandResponse>
{
    /// <summary>
    ///     DB Command for creating a Project.
    /// </summary>
    /// <param name="Name">Name of the project</param>
    public record Command(string Name) : IValidatableRequest<CommandResponse>
    {
        /// <inheritdoc />
        public (bool valid, string? invalidReason) IsRequestValid()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                return (false, "");
            }

            return (true, null);
        }
    }

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        EntityEntry<Tag> created = await dbContext.Tags.AddAsync(new()
        {
            Name = request.Name
        }, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return new()
        {
            Success = true,
            Id = created.Entity.Id
        };
    }
}