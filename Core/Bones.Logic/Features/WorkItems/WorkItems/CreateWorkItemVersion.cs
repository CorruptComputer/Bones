using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.GenericItems;
using Bones.Database.DbSets.WorkItemManagement;
using Bones.Database.Operations.WorkItemManagement.WorkItems;
using Bones.Logic.Features.GenericItem;
using Bones.Shared.Consts;

namespace Bones.Logic.Features.WorkItems.WorkItems;

/// <inheritdoc />
public sealed class CreateWorkItemVersion(ISender sender) : IRequestHandler<CreateWorkItemVersion.Command, CommandResponse>
{
    /// <summary>
    ///   Command for creating a Work Item in a Queue.
    /// </summary>
    /// <param name="WorkItemId"></param>
    /// <param name="WorkItemLayoutId"></param>
    /// <param name="Title">The title to use for this item</param>
    /// <param name="Values"></param>
    /// <param name="RequestingUser"></param>
    public sealed record Command(Guid WorkItemId, Guid WorkItemLayoutId, string Title, Dictionary<Guid, object?> Values, BonesUser RequestingUser) : IRequest<CommandResponse>;
    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Command>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.WorkItemId).NotNull().NotEqual(Guid.Empty);
            RuleFor(x => x.WorkItemLayoutId).NotNull().NotEqual(Guid.Empty);
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
        bool? permission = await sender.Send(new UserHasWorkItemPermission.Query(request.WorkItemId, request.RequestingUser, BonesClaimTypes.Role.WorkItem.CREATE_WORK_ITEM_VERSION), cancellationToken);
        if (permission != true)
        {
            return CommandResponse.Forbid();
        }

        GenericItemLayout? layout = await sender.Send(new GetItemLayoutById.Query(request.WorkItemLayoutId, request.RequestingUser), cancellationToken);
        if (layout?.LatestVersion is null)
        {
            return CommandResponse.Fail("Layout not found");
        }

        WorkItem? workItem = await sender.Send(new GetWorkItemByIdDb.Query(request.WorkItemId), cancellationToken);
        if (workItem is null)
        {
            return CommandResponse.Fail("Work Item not found");
        }

        return await sender.Send(new CreateWorkItemVersionDb.Command(workItem.Id, request.Title, layout.LatestVersion.Id, request.Values), cancellationToken);
    }
}