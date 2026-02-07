using Bones.Database.DbSets.Accounts;
using Bones.Database.DbSets.Items.Layouts;
using Bones.Database.DbSets.Items.Types;
using Bones.Database.Operations.Items.Types.Tasks;
using Bones.Logic.Features.Items;
using Bones.Shared.Consts;

namespace Bones.Logic.Features.Tasks.Tasks;

/// <inheritdoc />
public sealed class CreateTaskVersion(ISender sender) : IRequestHandler<CreateTaskVersion.Command, CommandResponse>
{
    /// <summary>
    ///   Command for creating a Task in a Queue.
    /// </summary>
    /// <param name="TaskId"></param>
    /// <param name="TaskLayoutId"></param>
    /// <param name="Title">The title to use for this item</param>
    /// <param name="Values"></param>
    /// <param name="ActionDateTime"></param>
    /// <param name="RequestingUser"></param>
    public sealed record Command(Guid TaskId, Guid TaskLayoutId, string Title, Dictionary<Guid, object?> Values, DateTimeOffset ActionDateTime, BonesUser RequestingUser) : IRequest<CommandResponse>;
    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Command>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.TaskId).NotNull().NotEqual(Guid.Empty);
            RuleFor(x => x.TaskLayoutId).NotNull().NotEqual(Guid.Empty);
            RuleFor(x => x.Title).NotEmpty().MaximumLength(256);
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
        bool? permission = await sender.Send(new UserHasTaskPermission.Query(request.TaskId, request.RequestingUser, BonesClaimTypes.Role.Task.CREATE_TASK_VERSION), cancellationToken);
        if (permission != true)
        {
            return CommandResponse.Forbid();
        }

        ItemLayout? layout = await sender.Send(new GetItemLayoutById.Query(request.TaskLayoutId, request.RequestingUser), cancellationToken);
        if (layout?.Current is null)
        {
            return CommandResponse.Fail("Layout not found");
        }

        BonesTask? task = await sender.Send(new GetTaskByIdDb.Query(request.TaskId), cancellationToken);
        if (task is null)
        {
            return CommandResponse.Fail("Task not found");
        }

        return await sender.Send(new CreateTaskVersionDb.Command(task.Id, request.Title, layout.Current.Id, request.Values, request.ActionDateTime), cancellationToken);
    }
}