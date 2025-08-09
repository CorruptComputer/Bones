using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.AssetManagement;
using Bones.Database.DbSets.GenericItems;
using Bones.Database.DbSets.WorkItemManagement;
using Bones.Database.Operations.AssetManagement;
using Bones.Database.Operations.GenericItem;
using Bones.Database.Operations.WorkItemManagement.WorkItemQueues;
using Bones.Database.Operations.WorkItemManagement.WorkItems;
using Bones.Logic.Features.Projects;
using Bones.Logic.Features.WorkItems.Queue;
using Bones.Shared.Consts;

namespace Bones.Logic.Features.Assets;

/// <inheritdoc />
public sealed class CreateAsset(ISender sender) : IRequestHandler<CreateAsset.Command, CommandResponse>
{
    /// <summary>
    ///   Command for creating an Asset.
    /// </summary>
    /// <param name="LayoutId"></param>
    /// <param name="LayoutVersionId"></param>
    /// <param name="Title">The title to use for this item</param>
    /// <param name="Values"></param>
    /// <param name="ActionDateTime"></param>
    /// <param name="RequestingUser"></param>
    public sealed record Command(Guid LayoutId, Guid LayoutVersionId, string Title, Dictionary<Guid, object?> Values, DateTimeOffset ActionDateTime, BonesUser RequestingUser) : IRequest<CommandResponse>;
    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Command>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.LayoutId).NotNull().NotEqual(Guid.Empty);
            RuleFor(x => x.LayoutVersionId).NotNull().NotEqual(Guid.Empty);
            RuleFor(x => x.Title).NotNull().NotEmpty().MaximumLength(256);
            RuleFor(x => x.Values).NotNull().ChildRules(dict =>
            {
                dict.RuleForEach(x => x.Keys).NotNull().NotEmpty();
            });
            RuleFor(x => x.ActionDateTime).NotNull().LessThanOrEqualTo(DateTimeOffset.UtcNow)
                .WithMessage("Action date time cannot be in the future");
            RuleFor(x => x.RequestingUser).NotNull();
        }
    }

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        GenericItemLayout? layout = await sender.Send(new GetItemLayoutByIdDb.Query(request.LayoutId), cancellationToken);
        if (layout is null)
        {
            return CommandResponse.Fail("Layout not found");
        }

        bool? permission = await sender.Send(new UserHasProjectPermission.Query(layout.Project.Id, request.RequestingUser, BonesClaimTypes.Role.Asset.CREATE_ASSET), cancellationToken);
        if (permission != true)
        {
            return CommandResponse.Forbid();
        }

        CommandResponse asset = await sender.Send(new CreateAssetDb.Command(request.LayoutId, request.ActionDateTime), cancellationToken);

        if (!asset.Success)
        {
            return asset;
        }

        CommandResponse itemVersion = await sender.Send(new CreateAssetVersionDb.Command(asset.Ids[nameof(Asset)], request.Title, request.LayoutVersionId, request.Values, request.ActionDateTime), cancellationToken);

        if (!itemVersion.Success)
        {
            return itemVersion;
        }

        Dictionary<string, Guid> ids = new()
        {
            { nameof(Asset), asset.Ids[nameof(Asset)] },
            { nameof(GenericItemVersion), itemVersion.Ids[nameof(GenericItemVersion)] }
        };

        return CommandResponse.Pass(ids);
    }
}