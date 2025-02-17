using Bones.Database.DbSets.AccountManagement;
using Bones.Database.Operations.WorkItemManagement.WorkItemQueues;

namespace Bones.Logic.Features.Projects.WorkItems;

/// <inheritdoc />
public sealed class CreateQueue(ISender sender) : IRequestHandler<CreateQueue.Command, CommandResponse>
{
    /// <summary>
    ///     Command for creating a Queue.
    /// </summary>
    /// <param name="Name">Name of the queue</param>
    /// <param name="InitiativeId">Internal ID of the initiative</param>
    /// <param name="RequestingUser"></param>
    public sealed record Command(string Name, Guid InitiativeId, BonesUser RequestingUser) : IRequest<CommandResponse>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Command>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.InitiativeId).NotNull().NotEqual(Guid.Empty);
            RuleFor(x => x.RequestingUser).NotNull();
        }
    }

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        // TODO: Check permission
        return await sender.Send(new CreateQueueDb.Command(request.Name, request.InitiativeId), cancellationToken);
    }
}
