using Bones.Database.Operations.WorkItemManagement.WorkItems;

namespace Bones.Logic.Features.Projects.WorkItems.CreateWorkItem;

internal sealed class CreateWorkItemHandler(ISender sender) : IRequestHandler<CreateWorkItemCommand, CommandResponse>
{
    public async Task<CommandResponse> Handle(CreateWorkItemCommand request, CancellationToken cancellationToken)
    {
        return await sender.Send(new CreateWorkItemDb.Command(request.Name, request.QueueId, request.ItemLayoutId), cancellationToken);
    }
}