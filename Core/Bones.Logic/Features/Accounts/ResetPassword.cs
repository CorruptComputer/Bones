using System;

namespace Bones.Logic.Features.Accounts;

/// <summary>
///   Request to reset password
/// </summary>
public sealed record ResetPasswordCommand : IRequest<CommandResponse>;

internal class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{

}

internal class ResetPasswordHandler : IRequestHandler<ResetPasswordCommand, CommandResponse>
{
    public Task<CommandResponse> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}