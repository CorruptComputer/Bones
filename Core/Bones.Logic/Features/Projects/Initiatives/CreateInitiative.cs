using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.OrganizationManagement;
using Bones.Database.Operations.OrganizationManagement.GetOrganizationByIdDb;
using Bones.Database.Operations.ProjectManagement.Initiatives.CreateInitiativeDb;
using Bones.Database.Operations.ProjectManagement.Projects;
using Bones.Logic.Features.Projects.Projects.UserHasProjectPermission;
using Bones.Shared.Backend.Enums;
using Bones.Shared.Consts;

namespace Bones.Logic.Features.Projects.Initiatives;

/// <summary>
///   Backend Command for creating an Initiative.
/// </summary>
/// <param name="Name">Name of the initiative</param>
/// <param name="ProjectId">Internal ID of the project</param>
/// <param name="RequestingUser">The user requesting this</param>
public record CreateInitiativeCommand(string Name, Guid ProjectId, BonesUser RequestingUser) : IRequest<CommandResponse>;

internal sealed class CreateInitiativeCommandValidator : AbstractValidator<CreateInitiativeCommand>
{

}

internal sealed class CreateInitiativeHandler(ISender sender) : IRequestHandler<CreateInitiativeCommand, CommandResponse>
{
    public async Task<CommandResponse> Handle(CreateInitiativeCommand request, CancellationToken cancellationToken)
    {
        Database.DbSets.ProjectManagement.Project? project = await sender.Send(new GetProjectByIdDbQuery(request.ProjectId), cancellationToken);

        if (project is null)
        {
            return CommandResponse.Fail("Project not found");
        }

        if (project.OwnerType == OwnershipType.User
            && project.OwningUser!.Id == request.RequestingUser.Id)
        {
            return await sender.Send(new CreateInitiativeDbCommand(request.Name, request.ProjectId), cancellationToken);
        }

        BonesOrganization? organization = await sender.Send(new GetOrganizationByIdDbQuery(project.OwningOrganization!.Id), cancellationToken);
        // Don't want to give away that this org doesn't exist, instead just return forbidden.
        if (organization is null)
        {
            return CommandResponse.Forbid();
        }

        const string perm = BonesClaimTypes.Role.Initiative.CREATE_INITIATIVE;
        bool? hasOrganizationPermission =
            await sender.Send(new UserHasProjectPermissionQuery(project.Id, request.RequestingUser, perm), cancellationToken);

        if (hasOrganizationPermission != true)
        {
            return CommandResponse.Forbid();
        }

        return await sender.Send(new CreateInitiativeDbCommand(request.Name, request.ProjectId), cancellationToken);
    }
}