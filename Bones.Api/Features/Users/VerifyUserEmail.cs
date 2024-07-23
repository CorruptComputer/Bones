using Bones.Database.Operations.Users;

namespace Bones.Api.Features.Users;

public class VerifyUserEmail(ISender sender) : IRequestHandler<VerifyUserEmail.Command, CommandResponse>
{
    /// <summary>
    /// </summary>
    /// <param name="UserId"></param>
    /// <param name="Token"></param>
    public record Command(Guid UserId, Guid Token) : IRequest<CommandResponse>;
    
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        return await sender.Send(new VerifyUserEmailDb.Command(request.UserId, request.Token), cancellationToken);
    }
}