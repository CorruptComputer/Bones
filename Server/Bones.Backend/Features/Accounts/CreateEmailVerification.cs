using Bones.Backend.Models;
using Bones.Database.Models;
using Bones.Database.Operations.Accounts;
using MediatR;

namespace Bones.Backend.Features.Accounts;

/// <summary>
///     Command for creating an email verification.
/// </summary>
/// <param name="AccountId">AccountId to require verification</param>
public record CreateEmailVerificationCommand(long AccountId) : IRequest<BackendCommandResponse>;

internal class CreateEmailVerificationHandler(ISender sender)
    : IRequestHandler<CreateEmailVerificationCommand, BackendCommandResponse>
{
    public async Task<BackendCommandResponse> Handle(CreateEmailVerificationCommand request,
        CancellationToken cancellationToken)
    {
        DbCommandResponse clearOldVerifications =
            await sender.Send(new ClearEmailVerificationsForAccountDbCommand(request.AccountId), cancellationToken);

        if (!clearOldVerifications.Success)
        {
            return clearOldVerifications;
        }

        DbCommandResponse createVerification =
            await sender.Send(new CreateEmailVerificationDbCommand(request.AccountId), cancellationToken);

        return createVerification;
    }
}