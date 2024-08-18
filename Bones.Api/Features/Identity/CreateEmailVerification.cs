using Bones.Database.Operations.Identity;

namespace Bones.Api.Features.Identity;

public class CreateEmailVerification(ISender sender) : IRequestHandler<CreateEmailVerification.Command, CommandResponse>
{
    /// <summary>
    ///     Command for creating an email verification.
    /// </summary>
    /// <param name="UserId">UserId to require verification</param>
    public record Command(Guid UserId) : IValidatableRequest<CommandResponse>
    {
        /// <inheritdoc />
        public (bool valid, string? invalidReason) IsRequestValid()
        {
            if (UserId == Guid.Empty)
            {
                return (false, "");
            }

            return (true, null);
        }
    }

    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        CommandResponse clearOldVerifications = await sender.Send(new ClearEmailVerificationsForUserDb.Command(request.UserId), cancellationToken);

        if (!clearOldVerifications.Success)
        {
            return clearOldVerifications;
        }

        CommandResponse createVerification = await sender.Send(new CreateEmailVerificationDb.Command(request.UserId), cancellationToken);

        return createVerification;
    }
}