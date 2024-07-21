using Bones.Database.Operations.Accounts;

namespace Bones.Api.Features.Accounts;

/// <summary>
/// </summary>
/// <param name="AccountId"></param>
/// <param name="Token"></param>
public record VerifyAccountEmailCommand(Guid AccountId, Guid Token) : IRequest<CommandResponse>;

internal class VerifyAccountEmailHandler(ISender sender)
    : IRequestHandler<VerifyAccountEmailCommand, CommandResponse>
{
    public async Task<CommandResponse> Handle(VerifyAccountEmailCommand request,
        CancellationToken cancellationToken)
    {
        return await sender.Send(new VerifyAccountEmailDbCommand(request.AccountId, request.Token), cancellationToken);
    }
}