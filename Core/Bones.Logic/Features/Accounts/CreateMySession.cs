using System.Net;
using Bones.Database.DbSets.AccountManagement;
using Bones.Database.Operations.AccountManagement;
using Bones.Shared.Backend;

namespace Bones.Logic.Features.Accounts;

/// <inheritdoc />
public sealed class CreateMySession(ISender sender) : IRequestHandler<CreateMySession.Query, QueryResponse<BonesUserSession?>>
{
    /// <summary>
    ///   Backend request for creating a session for the user
    /// </summary>
    /// <param name="RequestingIp"></param>
    /// <param name="RequestingUser"></param>
    public sealed record Query(IPAddress RequestingIp, BonesUser RequestingUser) : IRequest<QueryResponse<BonesUserSession?>>;

    /// <inheritdoc />
    public class Validator : AbstractValidator<Query>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.RequestingIp).NotEmpty();
            RuleFor(x => x.RequestingUser).NotNull();
        }
    }

    /// <inheritdoc />
    public async Task<QueryResponse<BonesUserSession?>> Handle(Query request, CancellationToken cancellationToken)
    {
        string base64LocalStorageKey = EncryptionHelper.GenerateAESKey();
        BonesUserSession? session = await sender.Send(new CreateAndGetBonesUserSessionDb.Query(request.RequestingUser, request.RequestingIp, base64LocalStorageKey), cancellationToken);
        
        return session;
    }
}
