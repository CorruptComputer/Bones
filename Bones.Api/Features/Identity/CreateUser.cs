using System.Text.RegularExpressions;
using Bones.Database.Operations.Identity;
using Bones.Shared.Extensions;

namespace Bones.Api.Features.Identity;

public partial class CreateUser(ISender sender) : IRequestHandler<CreateUser.Command, CommandResponse>
{
    /// <summary>
    ///     Command for creating a User.
    /// </summary>
    /// <param name="Email">Email address to use for the User.</param>
    public partial record Command(string Email, string Password) : IValidatableRequest<CommandResponse>
    {
        /// <inheritdoc />
        public (bool valid, string? invalidReason) IsRequestValid()
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                return (false, "Email is required.");
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                return (false, "Password is required.");
            }
            
            bool hasNumber = PasswordContainsNumber().IsMatch(Password);
            bool hasLower = PasswordContainsLower().IsMatch(Password);
            bool hasUpper = PasswordContainsUpper().IsMatch(Password);
            bool hasSpecial = PasswordContainsSpecial().IsMatch(Password);
            bool properLength = Password.Length >= 8;

            if (!hasNumber || !hasLower || !hasUpper || !hasSpecial || !properLength)
            {
                return (false, "Password not complex enough.");
            }

            return (true, null);
        }
        
        [GeneratedRegex(@"[0-9]")]
        private static partial Regex PasswordContainsNumber();
        
        [GeneratedRegex(@"[a-z]")]
        private static partial Regex PasswordContainsLower();
        
        [GeneratedRegex(@"[A-Z]")]
        private static partial Regex PasswordContainsUpper();

        [GeneratedRegex(@"[^a-zA-Z0-9]")]
        private static partial Regex PasswordContainsSpecial();
    }

    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        // Verify email is valid format, can't do this in the validate method since its async
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