namespace Bones.Backend.Features.AccountManagement.ResetPassword;

internal class ResetPasswordHandler : IRequestHandler<ResetPasswordRequest, CommandResponse>
{
    public Task<CommandResponse> Handle(ResetPasswordRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}