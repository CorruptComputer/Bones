namespace Bones.Backend.Features.AccountManagement.QueueForgotPasswordEmail;

internal class QueueForgotPasswordEmailHandler : IRequestHandler<QueueForgotPasswordEmailCommand, CommandResponse>
{
    public Task<CommandResponse> Handle(QueueForgotPasswordEmailCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}