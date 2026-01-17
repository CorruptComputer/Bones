using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.AssetManagement;
using Bones.Database.DbSets.Items;
using Bones.Database.Operations.AccountManagement;
using Bones.Database.Operations.AssetManagement;
using Bones.Database.Operations.Items;
using Bones.Shared.Backend.Enums;
using Bones.Shared.Consts;

namespace Bones.Logic.Features.Assets;

/// <inheritdoc />
public sealed class AssignAsset(ISender sender) : IRequestHandler<AssignAsset.Command, CommandResponse>
{
    /// <summary>
    ///   Command for assigning an asset to a user
    /// </summary>
    /// <param name="AssetId">Internal ID of the asset</param>
    /// <param name="AssignmentSlotId">ID of the assignment slot</param>
    /// <param name="BonesUserId">ID of the user to assign to</param>
    /// <param name="State">State for the assignment</param>
    /// <param name="RequestingUser">The user making the request</param>
    public record Command(Guid AssetId, Guid AssignmentSlotId, Guid BonesUserId, string State, BonesUser RequestingUser) : IRequest<CommandResponse>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Command>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.AssetId).NotNull().NotEqual(Guid.Empty);
            RuleFor(x => x.AssignmentSlotId).NotNull().NotEqual(Guid.Empty);
            RuleFor(x => x.BonesUserId).NotNull().NotEqual(Guid.Empty);
            RuleFor(x => x.State).NotEmpty();
        }
    }

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        // TODO: Implement proper asset permission check
        const string perm = BonesClaimTypes.Role.Asset.EDIT_ASSET;
        bool? hasAssetPermission =
            await sender.Send(new UserHasAssetPermission.Query(request.AssetId, request.RequestingUser, perm), cancellationToken);

        if (hasAssetPermission != true)
        {
            return CommandResponse.Forbid();
        }

        // Get the asset
        Asset? asset = await sender.Send(new GetAssetByIdDb.Query(request.AssetId, IncludeItem: true), cancellationToken);

        if (asset is null || asset.Item is null || asset.Item.Current is null)
        {
            return CommandResponse.Fail("Asset not found");
        }

        // Get the assignee slot
        ItemAssignmentSlot? assigneeSlot = asset.Item.Current.ItemLayoutVersion?.ItemAssignmentSlots.FirstOrDefault(x => x.Id == request.AssignmentSlotId);
        if (assigneeSlot is null)
        {
            return CommandResponse.Fail("Assignment slot not found");
        }

        // Check if the assignment slot is full
        if (assigneeSlot.SelectionType == SelectionType.Single)
        {
            bool isAlreadyAssigned = asset.Item.Current!.ItemAssignees.Any(x => x.ItemAssignmentSlotId == request.AssignmentSlotId);
            if (isAlreadyAssigned)
            {
                return CommandResponse.Fail("Assignment slot is already filled");
            }
        }

        // Check if the assignment slot accepts users
        if (assigneeSlot.AssignmentType != AssignmentType.User)
        {
            return CommandResponse.Fail("Assignment slot does not accept user assignees");
        }

        BonesUser? assigneeUser = await sender.Send(new GetUserByIdDb.Query(request.BonesUserId), cancellationToken);
        if (assigneeUser is null)
        {
            return CommandResponse.Fail("User not found");
        }

        // Check if the user being assigned has view permissions
        bool? hasViewPermission =
            await sender.Send(new UserHasAssetPermission.Query(request.AssetId, assigneeUser, BonesClaimTypes.Role.Asset.VIEW_ASSET), cancellationToken);

        if (hasViewPermission != true)
        {
            return CommandResponse.Fail("User does not have permission to view this asset");
        }

        CommandResponse assignAssetResponse = await sender.Send(
            new AssignItemDb.Command(asset.Item.Id, request.AssignmentSlotId, request.BonesUserId, request.State), cancellationToken);

        return assignAssetResponse;
    }
}