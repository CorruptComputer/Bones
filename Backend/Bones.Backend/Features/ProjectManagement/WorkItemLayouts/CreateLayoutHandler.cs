using Bones.Database.Operations.ProjectManagement.WorkItemLayouts.CreateWorkItemLayoutDb;

namespace Bones.Backend.Features.ProjectManagement.WorkItemLayouts;

internal sealed class CreateLayoutHandler(ISender sender) : IRequestHandler<CreateLayoutCommand, CommandResponse>
{
    public async Task<CommandResponse> Handle(CreateLayoutCommand request, CancellationToken cancellationToken)
    {
        return await sender.Send(new CreateWorkItemLayoutDbCommand(request.ProjectId, request.Name), cancellationToken);
    }
}