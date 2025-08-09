using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.AssetManagement;
using Bones.Database.Operations.AssetManagement;
using Bones.Shared.Consts;

namespace Bones.Logic.Features.Assets;

/// <inheritdoc />
public sealed class GetAssetsByLayoutId(ISender sender) : IRequestHandler<GetAssetsByLayoutId.Query, QueryResponse<List<Asset>>>
{
    /// <summary>
    ///   Command for creating a Queue.
    /// </summary>
    /// <param name="GenericItemLayoutId">Internal ID of the asset</param>
    /// <param name="RequestingUser"></param>
    public sealed record Query(Guid GenericItemLayoutId, BonesUser RequestingUser) : IRequest<QueryResponse<List<Asset>>>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Query>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.GenericItemLayoutId).NotNull().NotEqual(Guid.Empty);
            RuleFor(x => x.RequestingUser).NotNull();
        }
    }

    /// <inheritdoc />
    public async Task<QueryResponse<List<Asset>>> Handle(Query request, CancellationToken cancellationToken)
    {
        QueryResponse<List<Asset>> assets = await sender.Send(new GetAssetsByLayoutIdDb.Query(request.GenericItemLayoutId), cancellationToken);
        if (!assets.Success || assets.Result == null || assets.Result.Count == 0)
        {
            return assets;
        }

        foreach (Asset asset in assets.Result)
        {
            // No need to return forbidden for this, this should just filter out assets the user doesn't have permission to view
            bool? permission = await sender.Send(new UserHasAssetPermission.Query(asset.Id, request.RequestingUser, BonesClaimTypes.Role.Asset.VIEW_ASSET), cancellationToken);
            if (permission != true)
            {
                assets.Result.Remove(asset);
            }
        }

        return assets.Result;
    }
}