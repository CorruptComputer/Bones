using Bones.Database.Operations.WorkItemManagement.WorkItems;

namespace Bones.Logic.Features.Projects.WorkItems.CreateWorkItemVersion;

internal sealed class CreateWorkItemVersionHandler(ISender sender) : IRequestHandler<CreateWorkItemVersionCommand, CommandResponse>
{
    public async Task<CommandResponse> Handle(CreateWorkItemVersionCommand request, CancellationToken cancellationToken)
    {
        return await sender.Send(new CreateWorkItemVersionDb.Command(request.WorkItemId, request.WorkItemLayoutVersionId, request.Values), cancellationToken);
    }
}