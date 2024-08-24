using Bones.Shared.Backend.Models;

namespace Bones.Backend.Features.AccountManagement.QueueConfirmationEmail;

public class QueueConfirmationEmailHandler : IRequestHandler<QueueConfirmationEmailRequest, CommandResponse>
{
    public Task<CommandResponse> Handle(QueueConfirmationEmailRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}