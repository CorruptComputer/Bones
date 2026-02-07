using Bones.Database.DbSets.Accounts;
using Bones.Database.Operations.System.SystemSettings;

namespace Bones.Logic.Features.System.SystemSettings;

/// <inheritdoc />
public class GetBackgroundServiceUser(ISender sender) : IRequestHandler<GetBackgroundServiceUser.Query, QueryResponse<BonesUser?>>
{
    /// <summary>
    ///   Query to get the background service user
    /// </summary>
    public sealed record Query : IRequest<QueryResponse<BonesUser?>>;


    /// <inheritdoc />
    public async Task<QueryResponse<BonesUser?>> Handle(Query request, CancellationToken cancellationToken)
    {
        return await sender.Send(new GetBackgroundServiceUserDb.Query(), cancellationToken);
    }
}