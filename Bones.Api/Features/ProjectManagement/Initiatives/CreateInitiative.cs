using Bones.Database.Operations.ProjectManagement.Initiatives;

namespace Bones.Api.Features.ProjectManagement.Initiatives;

public class CreateInitiative(ISender sender) : IRequestHandler<CreateInitiative.Command, CommandResponse>
{
    /// <summary>
    ///     DB Command for creating an Initiative.
    /// </summary>
    /// <param name="Name">Name of the initiative</param>
    /// <param name="ProjectId">Internal ID of the project</param>
    public record Command(string Name, Guid ProjectId) : IValidatableRequest<CommandResponse>
    {
        /// <inheritdoc />
        public (bool valid, string? invalidReason) IsRequestValid()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                return (false, "");
            }

            if (ProjectId == Guid.Empty)
            {
                return (false, "");
            }

            return (true, null);
        }
    }

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        return await sender.Send(new CreateInitiativeDb.Command(request.Name, request.ProjectId), cancellationToken);
    }
}