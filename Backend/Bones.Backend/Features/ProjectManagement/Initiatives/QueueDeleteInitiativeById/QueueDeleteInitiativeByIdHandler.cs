using Bones.Database.Operations.ProjectManagement.Initiatives.QueueDeleteInitiativeByIdDb;

namespace Bones.Backend.Features.ProjectManagement.Initiatives.QueueDeleteInitiativeById;

internal sealed class QueueDeleteInitiativeByIdHandler(ISender sender) : IRequestHandler<QueueDeleteInitiativeByIdCommand, CommandResponse>
{
    public async Task<CommandResponse> Handle(QueueDeleteInitiativeByIdCommand request, CancellationToken cancellationToken)
    {
        // TODO: Validate user permission here
        return await sender.Send(new QueueDeleteInitiativeByIdDbCommand(request.InitiativeId), cancellationToken);
    }
}