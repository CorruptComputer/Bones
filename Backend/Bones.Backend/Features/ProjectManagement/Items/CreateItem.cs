using Bones.Database.Operations.ProjectManagement.Items;
using Bones.Shared.Backend.Models;

namespace Bones.Backend.Features.ProjectManagement.Items;

public class CreateItem(ISender sender) : IRequestHandler<CreateItem.Command, CommandResponse>
{
    /// <summary>
    ///     DB Command for creating an WorkItem.
    /// </summary>
    /// <param name="Name">Name of the item</param>
    /// <param name="QueueId">Internal ID of the queue this item is in</param>
    /// <param name="LayoutVersionId">Internal ID of the layout version this item is using</param>
    /// <param name="Values">Internal ID of the queue</param>
    public record Command(string Name, Guid QueueId, Guid LayoutVersionId, Dictionary<string, object> Values)
        : IValidatableRequest<CommandResponse>
    {
        /// <inheritdoc />
        public (bool valid, string? invalidReason) IsRequestValid()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                return (false, "");
            }

            if (QueueId == Guid.Empty)
            {
                return (false, "");
            }

            if (LayoutVersionId == Guid.Empty)
            {
                return (false, "");
            }

            return (true, null);
        }
    }

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        return await sender.Send(new CreateItemDb.Command(request.Name, request.QueueId, request.LayoutVersionId, request.Values), cancellationToken);
    }
}