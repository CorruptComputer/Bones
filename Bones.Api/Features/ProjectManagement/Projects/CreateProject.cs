using Bones.Database.DbSets.Identity;
using Bones.Database.Operations.ProjectManagement.Projects.CreateProjectDb;

namespace Bones.Api.Features.ProjectManagement.Projects;

public class CreateProject(ISender sender) : IRequestHandler<CreateProject.Command, CommandResponse>
{
    /// <summary>
    ///     DB Command for creating a Project.
    /// </summary>
    /// <param name="Name">Name of the project</param>
    public record Command(string Name, BonesUser RequestingUser) : IValidatableRequest<CommandResponse>
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
        return await sender.Send(new CreateProjectDbCommand(request.Name, request.RequestingUser), cancellationToken);
    }
}