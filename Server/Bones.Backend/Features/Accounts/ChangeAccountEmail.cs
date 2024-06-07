using Bones.Backend.Extensions;
using Bones.Backend.Models;
using Bones.Database.Models;
using Bones.Database.Operations.Accounts;
using MediatR;

namespace Bones.Backend.Features.Accounts;

/// <summary>
///     Command for changing the accounts Email Address.
/// </summary>
/// <param name="AccountId">Account ID of the account to change it on.</param>
/// <param name="Email">New email address to be used.</param>
public record ChangeAccountEmailCommand(long AccountId, string Email) : IRequest<BackendCommandResponse>;

internal class ChangeAccountEmailHandler(ISender sender)
    : IRequestHandler<ChangeAccountEmailCommand, BackendCommandResponse>
{
    public async Task<BackendCommandResponse> Handle(ChangeAccountEmailCommand request,
        CancellationToken cancellationToken)
    {
        // Verify email is valid format
        if (!request.Email.IsValidEmail())
        {
            return new()
            {
                Success = false,
                FailureReason = "Invalid email format."
            };
        }


        DbCommandResponse emailChanged =
            await sender.Send(new ChangeAccountEmailDbCommand(request.AccountId, request.Email));

        if (!emailChanged.Success)
        {
            return emailChanged;
        }

        BackendCommandResponse result =
            await sender.Send(new CreateEmailVerificationCommand(request.AccountId), cancellationToken);

        if (!result.Success)
        {
            return new()
            {
                Success = false,
                FailureReason =
                    $"Email updated, but failed to create verification email. Reason: {result.FailureReason}"
            };
        }

        return new()
        {
            Success = true
        };
    }
}