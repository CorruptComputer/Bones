using Bones.Database.Operations.ProjectManagement.WorkItems.CreateWorkItemVersionDb;

namespace Bones.Backend.Features.ProjectManagement.WorkItems.CreateWorkItemVersion;

internal sealed class CreateWorkItemVersionHandler(ISender sender) : IRequestHandler<CreateWorkItemVersionCommand, CommandResponse>
{
    public async Task<CommandResponse> Handle(CreateWorkItemVersionCommand request, CancellationToken cancellationToken)
    {
        return await sender.Send(new CreateWorkItemVersionDbCommand(request.WorkItemId, request.WorkItemLayoutVersionId, request.Values), cancellationToken);
    }
}