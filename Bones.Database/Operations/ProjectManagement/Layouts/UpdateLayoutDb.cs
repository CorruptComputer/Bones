using Bones.Database.DbSets.ProjectManagement.Layouts;

namespace Bones.Database.Operations.ProjectManagement.Layouts;

public sealed class UpdateLayoutDb(BonesDbContext dbContext) : IRequestHandler<UpdateLayoutDb.Command, CommandResponse>
{
    /// <summary>
    ///     DB Command for updating a Layout.
    /// </summary>
    /// <param name="LayoutId">Internal ID of the layout</param>
    /// <param name="Name">The new name of the layout</param>
    public record Command(Guid LayoutId, string Name) : IValidatableRequest<CommandResponse>
    {
        /// <inheritdoc />
        public (bool valid, string? invalidReason) IsRequestValid()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                return (false, "");
            }

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
        Layout? layout = await dbContext.Layouts.FirstOrDefaultAsync(p => p.Id == request.LayoutId, cancellationToken);
        if (layout == null)
        {
            return new()
            {
                Success = false,
                FailureReason = "Invalid LayoutId."
            };
        }

        layout.Name = request.Name;

        await dbContext.SaveChangesAsync(cancellationToken);

        return new()
        {
            Success = true
        };
    }
}