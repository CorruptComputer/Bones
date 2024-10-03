using Bones.Database.Operations.WorkItemManagement.WorkItemQueues.CreateQueueDb;

namespace Bones.Backend.Features.ProjectManagement.Queues.CreateQueue;

internal sealed class CreateQueueHandler(ISender sender) : IRequestHandler<CreateQueueCommand, CommandResponse>
{
    public async Task<CommandResponse> Handle(CreateQueueCommand request, CancellationToken cancellationToken)
    {
        // TODO: Check permission
        return await sender.Send(new CreateQueueDbCommand(request.Name, request.InitiativeId), cancellationToken);
    }
}