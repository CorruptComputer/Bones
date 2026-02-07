using Bones.Database.DbSets.Accounts;
using Bones.Database.Operations.System.SystemSettings;

namespace Bones.Logic.Features.System.SystemSettings;

/// <inheritdoc />
public class GetSystemAdminMaskUser(ISender sender) : IRequestHandler<GetSystemAdminMaskUser.Query, QueryResponse<BonesUser?>>
{
    /// <summary>
    ///   Query to get the system admin mask user
    /// </summary>
    public sealed record Query : IRequest<QueryResponse<BonesUser?>>;


    /// <inheritdoc />
    public async Task<QueryResponse<BonesUser?>> Handle(Query request, CancellationToken cancellationToken)
    {
        return await sender.Send(new GetSystemAdminMaskUserDb.Query(), cancellationToken);
    }
}