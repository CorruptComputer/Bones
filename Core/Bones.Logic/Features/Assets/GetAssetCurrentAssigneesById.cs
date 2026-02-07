using Bones.Database.DbSets.Accounts;
using Bones.Database.DbSets.Items.Assignments;
using Bones.Database.Operations.Items.Types.Assets;
using Bones.Shared.Consts;

namespace Bones.Logic.Features.Assets;

/// <inheritdoc />
public sealed class GetAssetCurrentAssigneesById(ISender sender) : IRequestHandler<GetAssetCurrentAssigneesById.Query, QueryResponse<List<ItemAssignee>?>>
{
    /// <summary>
    ///   Retrieves the assignees for the current version of an asset.
    /// </summary>
    /// <param name="AssetId">ID of the asset</param>
    /// <param name="RequestingUser">The user requesting this data</param>
    public sealed record Query(Guid AssetId, BonesUser RequestingUser) : IRequest<QueryResponse<List<ItemAssignee>?>>;

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
    public async Task<QueryResponse<List<ItemAssignee>?>> Handle(Query request, CancellationToken cancellationToken)
    {
        bool? permission = await sender.Send(new UserHasAssetPermission.Query(request.AssetId, request.RequestingUser, BonesClaimTypes.Role.Asset.VIEW_ASSET), cancellationToken);
        if (permission != true)
        {
            return QueryResponse<List<ItemAssignee>?>.Forbid();
        }

        List<ItemAssignee>? assignees = await sender.Send(new GetAssetCurrentAssigneesByIdDb.Query(request.AssetId), cancellationToken);
        if (assignees is null)
        {
            return QueryResponse<List<ItemAssignee>?>.Fail("Assignees not found");
        }

        return assignees;
    }
}
