using Bones.Database.Operations.Accounts;
using Bones.Shared.Extensions;

namespace Bones.Api.Features.Accounts;

/// <summary>
///     Command for creating an account.
/// </summary>
/// <param name="Email">Email address to use for the account.</param>
public record CreateAccountCommand(string Email) : IRequest<CommandResponse>;

internal class CreateAccountHandler(ISender sender) : IRequestHandler<CreateAccountCommand, CommandResponse>
{
    public async Task<CommandResponse> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
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


        // Verify no other users in the DB have this email already
        CommandResponse emailAvailable =
            await sender.Send(new EmailAvailableForUseDbCommand(request.Email), cancellationToken);
        if (!emailAvailable.Success)
        {
            return emailAvailable;
        }

        // Create the account
        CommandResponse createAccount =
            await sender.Send(new CreateAccountDbCommand(request.Email), cancellationToken);

        if (!createAccount.Success || !createAccount.Id.HasValue)
        {
            return createAccount;
        }

        CommandResponse emailVerification =
            await sender.Send(new CreateEmailVerificationCommand(createAccount.Id.Value), cancellationToken);

        if (!emailVerification.Success)
        {
            return new()
            {
                Success = false,
                Id = createAccount.Id.Value,
                FailureReason =
                    $"Account created, but failed to create verification email. Reason: {emailVerification.FailureReason}"
            };
        }

        return new()
        {
            Success = true,
            Id = createAccount.Id.Value
        };
    }
}