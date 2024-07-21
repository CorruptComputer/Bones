using Bones.Database.Operations.Accounts;

namespace Bones.Api.Features.Accounts;

/// <summary>
///     Command for creating an email verification.
/// </summary>
/// <param name="AccountId">AccountId to require verification</param>
public record CreateEmailVerificationCommand(Guid AccountId) : IRequest<CommandResponse>;

internal class CreateEmailVerificationHandler(ISender sender)
    : IRequestHandler<CreateEmailVerificationCommand, CommandResponse>
{
    public async Task<CommandResponse> Handle(CreateEmailVerificationCommand request,
        CancellationToken cancellationToken)
    {
        CommandResponse clearOldVerifications =
            await sender.Send(new ClearEmailVerificationsForAccountDbCommand(request.AccountId), cancellationToken);

        if (!clearOldVerifications.Success)
        {
            return clearOldVerifications;
        }

        CommandResponse createVerification =
            await sender.Send(new CreateEmailVerificationDbCommand(request.AccountId), cancellationToken);

        return createVerification;
    }
}