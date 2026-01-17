using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.AssetManagement;
using Bones.Database.DbSets.Items;
using Bones.Database.Operations.AssetManagement;
using Bones.Shared.Consts;

namespace Bones.Logic.Features.Assets;

/// <inheritdoc />
public sealed class GetAssetAssigneeSlotsById(ISender sender) : IRequestHandler<GetAssetAssigneeSlotsById.Query, QueryResponse<List<ItemAssignmentSlot>?>>
{
    /// <summary>
    ///   Retrieves the assignee slots for the current version of an asset.
    /// </summary>
    /// <param name="AssetId">ID of the asset</param>
    /// <param name="RequestingUser">The user requesting this data</param>
    public sealed record Query(Guid AssetId, BonesUser RequestingUser) : IRequest<QueryResponse<List<ItemAssignmentSlot>?>>;

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
    public async Task<QueryResponse<List<ItemAssignmentSlot>?>> Handle(Query request, CancellationToken cancellationToken)
    {
        bool? permission = await sender.Send(new UserHasAssetPermission.Query(request.AssetId, request.RequestingUser, BonesClaimTypes.Role.Asset.VIEW_ASSET), cancellationToken);
        if (permission != true)
        {
            return QueryResponse<List<ItemAssignmentSlot>?>.Forbid();
        }

        Asset? asset = await sender.Send(new GetAssetByIdDb.Query(request.AssetId, IncludeItem: true), cancellationToken);
        if (asset is null || asset.Item is null || asset.Item.Current is null)
        {
            return QueryResponse<List<ItemAssignmentSlot>?>.Fail("Asset not found");
        }

        return asset.Item.Current.ItemLayoutVersion?.ItemAssignmentSlots;
    }
}
