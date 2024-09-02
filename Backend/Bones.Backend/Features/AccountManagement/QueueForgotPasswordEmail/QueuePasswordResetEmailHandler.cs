namespace Bones.Backend.Features.AccountManagement.QueueForgotPasswordEmail;

internal class QueueForgotPasswordHandler : IRequestHandler<QueueForgotPasswordRequest, CommandResponse>
{
    public Task<CommandResponse> Handle(QueueForgotPasswordRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}