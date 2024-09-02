namespace Bones.Backend.Features.ProjectManagement.Initiatives.CreateInitiative;

/// <summary>
///   Backend Command for creating an Initiative.
/// </summary>
/// <param name="Name">Name of the initiative</param>
/// <param name="ProjectId">Internal ID of the project</param>
public record CreateInitiativeCommand(string Name, Guid ProjectId) : IRequest<CommandResponse>;

internal class CreateInitiative(ISender sender) : IRequestHandler<CreateInitiativeCommand, CommandResponse>
{
    /// <inheritdoc />
    public async Task<CommandResponse> Handle(CreateInitiativeCommand request, CancellationToken cancellationToken)
    {
        return await sender.Send(new CreateInitiativeDbHandler.Command(request.Name, request.ProjectId), cancellationToken);
    }
}