using Bones.Database.Operations.ProjectManagement.Items;

namespace Bones.Api.Features.ProjectManagement.Items;

public class CreateTag(ISender sender) : IRequestHandler<CreateTag.Command, CommandResponse>
{
    /// <summary>
    ///     DB Command for creating a Project.
    /// </summary>
    /// <param name="Name">Name of the project</param>
    public record Command(string Name) : IValidatableRequest<CommandResponse>
    {
        /// <inheritdoc />
        public (bool valid, string? invalidReason) IsRequestValid()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                return (false, "");
            }

            return (true, null);
        }
    }

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        return await sender.Send(new CreateTagDb.Command(request.Name), cancellationToken);
    }
}