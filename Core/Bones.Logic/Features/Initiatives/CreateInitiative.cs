using Bones.Database.DbSets.Accounts;
using Bones.Database.DbSets.Organizations;
using Bones.Database.Operations.Organizations;
using Bones.Database.Operations.Projects.Initiatives;
using Bones.Database.Operations.Projects.Projects;
using Bones.Logic.Features.Projects;
using Bones.Shared.Backend.Enums;
using Bones.Shared.Consts;

namespace Bones.Logic.Features.Initiatives;

/// <inheritdoc />
public sealed class CreateInitiative(ISender sender) : IRequestHandler<CreateInitiative.Command, CommandResponse>
{
    /// <summary>
    ///   Backend Command for creating an Initiative.
    /// </summary>
    /// <param name="Name">Name of the initiative</param>
    /// <param name="ProjectId">Internal ID of the project</param>
    /// <param name="RequestingUser">The user requesting this</param>
    public record Command(string Name, Guid ProjectId, BonesUser RequestingUser) : IRequest<CommandResponse>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Command>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.ProjectId).NotNull().NotEqual(Guid.Empty);
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.RequestingUser).NotNull();
        }
    }

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        Database.DbSets.Projects.Project? project = await sender.Send(new GetProjectByIdDb.Query(request.ProjectId), cancellationToken);

        if (project is null)
        {
            return CommandResponse.Fail("Project not found");
        }

        if (project.OwnerType == OwnershipType.User
            && project.OwningUser!.Id == request.RequestingUser.Id)
        {
            return await sender.Send(new CreateInitiativeDb.Command(request.Name, request.ProjectId), cancellationToken);
        }

        BonesOrganization? organization = await sender.Send(new GetOrganizationByIdDb.Query(project.OwningOrganization!.Id), cancellationToken);
        // Don't want to give away that this org doesn't exist, instead just return forbidden.
        if (organization is null)
        {
            return CommandResponse.Forbid();
        }

        const string perm = BonesClaimTypes.Role.Initiative.CREATE_INITIATIVE;
        bool? hasOrganizationPermission =
            await sender.Send(new UserHasProjectPermission.Query(project.Id, request.RequestingUser, perm), cancellationToken);

        if (hasOrganizationPermission != true)
        {
            return CommandResponse.Forbid();
        }

        return await sender.Send(new CreateInitiativeDb.Command(request.Name, request.ProjectId), cancellationToken);
    }
}