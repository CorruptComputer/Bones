using Bones.Database.Operations.ProjectManagement.Tags.CreateTagDb;

namespace Bones.Backend.Features.ProjectManagement.Tags.CreateTag;

internal sealed class CreateTagHandler(ISender sender) : IRequestHandler<CreateTagCommand, CommandResponse>
{
    public async Task<CommandResponse> Handle(CreateTagCommand request, CancellationToken cancellationToken)
    {
        return await sender.Send(new CreateTagDbCommand(request.Name), cancellationToken);
    }
}