using Bones.Database.Operations.ProjectManagement.Queues;

namespace Bones.Api.Features.ProjectManagement.Queues;

public class CreateQueue(ISender sender) : IRequestHandler<CreateQueue.Command, CommandResponse>
{
    /// <summary>
    ///     DB Command for creating a Queue.
    /// </summary>
    /// <param name="Name">Name of the queue</param>
    /// <param name="InitiativeId">Internal ID of the initiative</param>
    public record Command(string Name, Guid InitiativeId) : IValidatableRequest<CommandResponse>
    {
        /// <inheritdoc />
        public bool IsRequestValid()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                return false;
            }

            if (InitiativeId == Guid.Empty)
            {
                return false;
            }

            return true;
        }
    }

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        return await sender.Send(new CreateQueueDb.Command(request.Name, request.InitiativeId), cancellationToken);
    }
}