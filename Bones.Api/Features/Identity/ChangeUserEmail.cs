using Bones.Database.Operations.Identity;
using Bones.Shared.Extensions;

namespace Bones.Api.Features.Identity;

public class ChangeUserEmail(ISender sender) : IRequestHandler<ChangeUserEmail.Command, CommandResponse>
{
    /// <summary>
    ///     Command for changing the Users email address.
    /// </summary>
    /// <param name="UserId">User ID of the User to change it on.</param>
    /// <param name="Email">New email address to be used.</param>
    public record Command(Guid UserId, string Email) : IValidatableRequest<CommandResponse>
    {
        /// <inheritdoc />
        public bool IsRequestValid()
        {
            if (UserId == Guid.Empty)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(Email))
            {
                return false;
            }

            return true;
        }
    }

    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        // Verify email is valid format
        if (!await request.Email.IsValidEmailAsync())
        {
            return new()
            {
                Success = false,
                FailureReason = "Invalid email."
            };
        }

        CommandResponse emailChanged = await sender.Send(new ChangeUserEmailDb.Command(request.UserId, request.Email), cancellationToken);

        if (!emailChanged.Success)
        {
            return emailChanged;
        }

        CommandResponse result = await sender.Send(new CreateEmailVerification.Command(request.UserId), cancellationToken);

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