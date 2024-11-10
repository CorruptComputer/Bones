namespace Bones.Backend.Features.Accounts.ResetPassword;

internal class ResetPasswordHandler : IRequestHandler<ResetPasswordCommand, CommandResponse>
{
    public Task<CommandResponse> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}