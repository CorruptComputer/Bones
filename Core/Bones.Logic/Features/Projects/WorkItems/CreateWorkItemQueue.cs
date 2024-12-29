using Bones.Database.DbSets.AccountManagement;
using Bones.Database.Operations.WorkItemManagement.WorkItemQueues.CreateQueueDb;

namespace Bones.Logic.Features.Projects.WorkItems;

/// <summary>
///     Command for creating a Queue.
/// </summary>
/// <param name="Name">Name of the queue</param>
/// <param name="InitiativeId">Internal ID of the initiative</param>
/// <param name="RequestingUser"></param>
public sealed record CreateQueueCommand(string Name, Guid InitiativeId, BonesUser RequestingUser) : IRequest<CommandResponse>;

internal sealed class CreateQueueCommandValidator : AbstractValidator<CreateQueueCommand>
{

}

internal sealed class CreateQueueHandler(ISender sender) : IRequestHandler<CreateQueueCommand, CommandResponse>
{
    public async Task<CommandResponse> Handle(CreateQueueCommand request, CancellationToken cancellationToken)
    {
        // TODO: Check permission
        return await sender.Send(new CreateQueueDbCommand(request.Name, request.InitiativeId), cancellationToken);
    }
}
