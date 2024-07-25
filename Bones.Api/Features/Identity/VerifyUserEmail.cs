using Bones.Database.Operations.Identity;

namespace Bones.Api.Features.Identity;

public class VerifyUserEmail(ISender sender) : IRequestHandler<VerifyUserEmail.Command, CommandResponse>
{
    /// <summary>
    /// </summary>
    /// <param name="UserId"></param>
    /// <param name="Token"></param>
    public record Command(Guid UserId, Guid Token) : IValidatableRequest<CommandResponse>
    {
        /// <inheritdoc />
        public bool IsRequestValid()
        {
            if (UserId == Guid.Empty)
            {
                return false;
            }

            if (Token == Guid.Empty)
            {
                return false;
            }

            return true;
        }
    }

    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        return await sender.Send(new VerifyUserEmailDb.Command(request.UserId, request.Token), cancellationToken);
    }
}