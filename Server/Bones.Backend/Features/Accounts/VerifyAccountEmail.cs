using Bones.Backend.Models;
using Bones.Database.Operations.Accounts;
using MediatR;

namespace Bones.Backend.Features.Accounts;

/// <summary>
/// </summary>
/// <param name="AccountId"></param>
/// <param name="Token"></param>
public record VerifyAccountEmailCommand(long AccountId, Guid Token) : IRequest<BackendCommandResponse>;

internal class VerifyAccountEmailHandler(ISender sender)
    : IRequestHandler<VerifyAccountEmailCommand, BackendCommandResponse>
{
    public async Task<BackendCommandResponse> Handle(VerifyAccountEmailCommand request,
        CancellationToken cancellationToken)
    {
        return await sender.Send(new VerifyAccountEmailDbCommand(request.AccountId, request.Token), cancellationToken);
    }
}