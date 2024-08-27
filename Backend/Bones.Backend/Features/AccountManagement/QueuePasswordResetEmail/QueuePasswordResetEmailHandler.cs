using Bones.Shared.Backend.Models;

namespace Bones.Backend.Features.AccountManagement.QueuePasswordResetEmail;

public class QueuePasswordResetHandler : IRequestHandler<QueuePasswordResetRequest, CommandResponse>
{
    public Task<CommandResponse> Handle(QueuePasswordResetRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}