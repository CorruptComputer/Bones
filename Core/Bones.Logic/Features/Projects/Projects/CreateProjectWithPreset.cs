using Bones.Database.DbSets.AccountManagement;
using Bones.Logic.Features.Projects.ProjectPresets;
using Bones.Shared.Enums;

namespace Bones.Logic.Features.Projects.Projects;

/// <inheritdoc />
public sealed class CreateProjectWithPreset(ISender sender) : IRequestHandler<CreateProjectWithPreset.Command, CommandResponse>
{
    /// <summary>
    ///     Command for creating a Project with a preset.
    /// </summary>
    /// <param name="Name">Name of the project</param>
    /// <param name="Preset">The preset to use</param>
    /// <param name="RequestingUser">The user requesting this project be created</param>
    /// <param name="OrganizationId">Optionally, the organization this project should belong to.</param>
    public record Command(string Name, ProjectPreset Preset, BonesUser RequestingUser, Guid? OrganizationId = null) : IRequest<CommandResponse>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Command>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.Name).NotNull().NotEmpty();
            RuleFor(x => x.Preset).NotNull();
            RuleFor(x => x.RequestingUser).NotNull();
        }
    }

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        CommandResponse project = await sender.Send(new CreateProject.Command(request.Name, request.RequestingUser, request.OrganizationId), cancellationToken);
        if (project.Success && project.Id.HasValue)
        {
            IProjectPreset? preset = request.Preset switch
            {
                ProjectPreset.Development => new DevelopmentPreset(),
                //ProjectPreset.InformationTechnology => new InformationTechnologyPreset(),
                //ProjectPreset.HomeManagement => new HomeManagementPreset(),
                _ => null
            };

            if (preset != null)
            {
                bool success = await preset.CreatePresetLayoutsAsync(sender, project.Id.Value, request.RequestingUser, cancellationToken);
                if (!success)
                {
                    return CommandResponse.Fail($"Failed to create layouts with preset: {request.Preset}");
                }
            }
            else
            {
                return CommandResponse.Fail($"Preset not found or unsupported: {request.Preset}");
            }
        }
        else
        {
            return CommandResponse.Fail($"Failed to create project: {project.FailureReasons}");
        }

        return CommandResponse.Pass(project.Id);
    }
}