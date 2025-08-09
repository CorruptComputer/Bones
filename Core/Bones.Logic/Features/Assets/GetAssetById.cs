using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.AssetManagement;
using Bones.Database.Operations.AssetManagement;
using Bones.Shared.Consts;

namespace Bones.Logic.Features.Assets;

/// <inheritdoc />
public sealed class GetAssetById(ISender sender) : IRequestHandler<GetAssetById.Query, QueryResponse<Asset?>>
{
    /// <summary>
    ///   Command for creating a Queue.
    /// </summary>
    /// <param name="AssetId">Internal ID of the asset</param>
    /// <param name="RequestingUser"></param>
    public sealed record Query(Guid AssetId, BonesUser RequestingUser) : IRequest<QueryResponse<Asset?>>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Query>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.AssetId).NotNull().NotEqual(Guid.Empty);
            RuleFor(x => x.RequestingUser).NotNull();
        }
    }

    /// <inheritdoc />
    public async Task<QueryResponse<Asset?>> Handle(Query request, CancellationToken cancellationToken)
    {
        bool? permission = await sender.Send(new UserHasAssetPermission.Query(request.AssetId, request.RequestingUser, BonesClaimTypes.Role.Asset.VIEW_ASSET), cancellationToken);
        if (permission != true)
        {
            return QueryResponse<Asset?>.Forbid();
        }

        return await sender.Send(new GetAssetByIdDb.Query(request.AssetId), cancellationToken);
    }
}