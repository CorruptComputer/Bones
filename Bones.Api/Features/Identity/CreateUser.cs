using Bones.Database.Operations.Identity;
using Bones.Shared.Extensions;

namespace Bones.Api.Features.Identity;

public class CreateUser(ISender sender) : IRequestHandler<CreateUser.Command, CommandResponse>
{
    /// <summary>
    ///     Command for creating a User.
    /// </summary>
    /// <param name="Email">Email address to use for the User.</param>
    public record Command(string Email) : IValidatableRequest<CommandResponse>
    {
        /// <inheritdoc />
        public bool IsRequestValid()
        {
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


        // Verify no other users in the DB have this email already
        CommandResponse emailAvailable = await sender.Send(new EmailAvailableForUseDb.Command(request.Email), cancellationToken);
        if (!emailAvailable.Success)
        {
            return emailAvailable;
        }

        // Create the User
        CommandResponse createUser = await sender.Send(new CreateUserDb.Command(request.Email), cancellationToken);

        if (!createUser.Success || !createUser.Id.HasValue)
        {
            return createUser;
        }

        CommandResponse emailVerification = await sender.Send(new CreateEmailVerification.Command(createUser.Id.Value), cancellationToken);

        if (!emailVerification.Success)
        {
            return new()
            {
                Success = false,
                Id = createUser.Id.Value,
                FailureReason =
                    $"User created, but failed to create verification email. Reason: {emailVerification.FailureReason}"
            };
        }

        return new()
        {
            Success = true,
            Id = createUser.Id.Value
        };
    }
}