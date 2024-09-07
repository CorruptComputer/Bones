namespace Bones.Backend.Features.AccountManagement.ResetPassword;

internal class ResetPasswordHandler : IRequestHandler<ResetPasswordCommand, CommandResponse>
{
    public Task<CommandResponse> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}