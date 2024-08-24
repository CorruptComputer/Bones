using Bones.Database.DbSets.ProjectManagement;
using Bones.Shared.Backend.Models;

namespace Bones.Database.Operations.ProjectManagement.Layouts;

public sealed class DeleteLayoutDb(BonesDbContext dbContext) : IRequestHandler<DeleteLayoutDb.Command, CommandResponse>
{
    /// <summary>
    ///     DB Command for deleting a Layout.
    /// </summary>
    /// <param name="LayoutId">Internal ID of the layout</param>
    public record Command(Guid LayoutId) : IValidatableRequest<CommandResponse>
    {
        /// <inheritdoc />
        public (bool valid, string? invalidReason) IsRequestValid()
        {
            if (LayoutId == Guid.Empty)
            {
                return (false, "");
            }

            return (true, null);
        }
    }

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        WorkItemLayout? layout = await dbContext.WorkItemLayouts.Include(layout => layout.Versions).FirstOrDefaultAsync(p => p.Id == request.LayoutId, cancellationToken);
        if (layout == null)
        {
            return new()
            {
                Success = false,
                FailureReason = "Invalid InitiativeId."
            };
        }

        foreach (WorkItemLayoutVersion version in layout.Versions)
        {
            // TODO: send it
            version.DeleteFlag = true;
        }

        layout.DeleteFlag = true;

        await dbContext.SaveChangesAsync(cancellationToken);

        return new()
        {
            Success = true
        };
    }
}