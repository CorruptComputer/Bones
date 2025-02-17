namespace Bones.Logic.Features.Accounts;

/// <inheritdoc />
public class ResetPassword : IRequestHandler<ResetPassword.Command, CommandResponse>
{
    /// <summary>
    ///   Request to reset password
    /// </summary>
    public sealed record Command : IRequest<CommandResponse>;

    /// <inheritdoc />
    public class Validator : AbstractValidator<Command>
    {

    }

    /// <inheritdoc />
    public Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}