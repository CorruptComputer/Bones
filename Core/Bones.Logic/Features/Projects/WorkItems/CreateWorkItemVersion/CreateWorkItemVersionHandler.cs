using Bones.Database.Operations.WorkItemManagement.WorkItems.CreateWorkItemVersionDb;

namespace Bones.Logic.Features.Projects.WorkItems.CreateWorkItemVersion;

internal sealed class CreateWorkItemVersionHandler(ISender sender) : IRequestHandler<CreateWorkItemVersionCommand, CommandResponse>
{
    public async Task<CommandResponse> Handle(CreateWorkItemVersionCommand request, CancellationToken cancellationToken)
    {
        return await sender.Send(new CreateWorkItemVersionDbCommand(request.WorkItemId, request.WorkItemLayoutVersionId, request.Values), cancellationToken);
    }
}