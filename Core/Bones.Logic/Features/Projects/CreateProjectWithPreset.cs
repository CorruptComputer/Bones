using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.ProjectManagement;
using Bones.Logic.Features.Projects.Presets;
using Bones.Shared.Backend.Enums;

namespace Bones.Logic.Features.Projects;

/// <inheritdoc />
public sealed class CreateProjectWithPreset(ISender sender) : IRequestHandler<CreateProjectWithPreset.Command, CommandResponse>
{
    /// <summary>
    ///   Command for creating a Project with a preset.
    /// </summary>
    /// <param name="Name">Name of the project</param>
    /// <param name="Preset">The preset to use</param>
    /// <param name="RequestingUser">The user requesting this project be created</param>
    /// <param name="OrganizationId">Optionally, the organization this project should belong to.</param>
    /// <param name="CreateTasks">Optionally, whether to create tasks for the project.</param>
    public record Command(string Name, ProjectPreset Preset, BonesUser RequestingUser, Guid? OrganizationId = null, bool CreateTasks = false) : IRequest<CommandResponse>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Command>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Preset).NotNull();
            RuleFor(x => x.RequestingUser).NotNull();
        }
    }

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        PresetBase? preset = request.Preset switch
        {
            ProjectPreset.Development => new DevelopmentPreset(),
            //ProjectPreset.InformationTechnology => new InformationTechnologyPreset(),
            //ProjectPreset.HomeManagement => new HomeManagementPreset(),
            ProjectPreset.Test => new TestPreset(),
            _ => null
        };

        Guid? projectId;
        if (preset != null)
        {
            projectId = await preset.CreatePresetAsync(sender, request.CreateTasks, request.RequestingUser, cancellationToken);
            if (projectId is null)
            {
                return CommandResponse.Fail($"Failed to create preset: {preset.GetType().FullName}");
            }
        }
        else
        {
            return CommandResponse.Fail($"Preset not found or unsupported: {request.Preset}");
        }

        return CommandResponse.Pass(nameof(Project), projectId.Value);
    }
}