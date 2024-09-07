using Bones.Database.Operations.ProjectManagement.WorkItemLayouts.CreateWorkItemLayoutDb;

namespace Bones.Backend.Features.ProjectManagement.Layouts;

public class CreateLayout(ISender sender) : IRequestHandler<CreateLayout.Command, CommandResponse>
{
    /// <summary>
    ///     DB Command for creating a layout.
    /// </summary>
    /// <param name="Name">Name of the layout</param>
    /// <param name="ItemFieldIds">Internal IDs of the fields this layout is using</param>
    public record Command(string Name, List<Guid> ItemFieldIds) : IValidatableRequest<CommandResponse>
    {
        /// <inheritdoc />
        public (bool valid, string? invalidReason) IsRequestValid()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                return (false, "");
            }

            foreach (Guid id in ItemFieldIds)
            {
                if (id == Guid.Empty)
                {
                    return (false, "");
                }
            }

            return (true, null);
        }
    }

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        return await sender.Send(new CreateWorkItemLayoutDbCommand(request.Name, request.ItemFieldIds), cancellationToken);
    }
}