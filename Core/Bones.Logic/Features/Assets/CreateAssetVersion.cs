using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.AssetManagement;
using Bones.Database.DbSets.Items;
using Bones.Database.DbSets.WorkItemManagement;
using Bones.Database.Operations.AssetManagement;
using Bones.Database.Operations.WorkItemManagement.WorkItems;
using Bones.Logic.Features.Item;
using Bones.Shared.Consts;

namespace Bones.Logic.Features.Assets;

/// <inheritdoc />
public sealed class CreateAssetVersion(ISender sender) : IRequestHandler<CreateAssetVersion.Command, CommandResponse>
{
    /// <summary>
    ///   Command for creating a Work Item in a Queue.
    /// </summary>
    /// <param name="AssetId"></param>
    /// <param name="LayoutId"></param>
    /// <param name="Title">The title to use for this item</param>
    /// <param name="Values"></param>
    /// <param name="ActionDateTime"></param>
    /// <param name="RequestingUser"></param>
    public sealed record Command(Guid AssetId, Guid LayoutId, string Title, Dictionary<Guid, object?> Values, DateTimeOffset ActionDateTime, BonesUser RequestingUser) : IRequest<CommandResponse>;
    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Command>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.AssetId).NotNull().NotEqual(Guid.Empty);
            RuleFor(x => x.LayoutId).NotNull().NotEqual(Guid.Empty);
            RuleFor(x => x.Title).NotNull().NotEmpty().MaximumLength(256);
            RuleFor(x => x.Values).NotNull().ChildRules(dict =>
            {
                dict.RuleForEach(x => x.Keys).NotNull().NotEqual(Guid.Empty);
            });
            RuleFor(x => x.RequestingUser).NotNull();
        }
    }

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        bool? permission = await sender.Send(new UserHasAssetPermission.Query(request.AssetId, request.RequestingUser, BonesClaimTypes.Role.Asset.CREATE_ASSET_VERSION), cancellationToken);
        if (permission != true)
        {
            return CommandResponse.Forbid();
        }

        ItemLayout? layout = await sender.Send(new GetItemLayoutById.Query(request.LayoutId, request.RequestingUser), cancellationToken);
        if (layout?.LatestVersion is null)
        {
            return CommandResponse.Fail("Layout not found");
        }

        Asset? asset = await sender.Send(new GetAssetByIdDb.Query(request.AssetId), cancellationToken);
        if (asset is null)
        {
            return CommandResponse.Fail("Asset not found");
        }

        return await sender.Send(new CreateAssetVersionDb.Command(asset.Id, request.Title, layout.LatestVersion.Id, request.Values, request.ActionDateTime), cancellationToken);
    }
}