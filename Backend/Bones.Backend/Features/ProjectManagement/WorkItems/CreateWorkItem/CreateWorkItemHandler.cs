using Bones.Database.Operations.ProjectManagement.WorkItems.CreateWorkItemDb;

namespace Bones.Backend.Features.ProjectManagement.WorkItems.CreateWorkItem;

internal sealed class CreateWorkItemHandler(ISender sender) : IRequestHandler<CreateWorkItemCommand, CommandResponse>
{
    public async Task<CommandResponse> Handle(CreateWorkItemCommand request, CancellationToken cancellationToken)
    {
        return await sender.Send(new CreateWorkItemDbCommand(request.Name, request.QueueId, request.WorkItemLayoutVersionId, request.Values), cancellationToken);
    }
}