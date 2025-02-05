using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.OrganizationManagement;
using Bones.Database.Operations.OrganizationManagement;
using Bones.Database.Operations.ProjectManagement.Projects.CreateProjectDb;
using Bones.Logic.Features.Organizations;
using Bones.Shared.Consts;

namespace Bones.Logic.Features.Projects.Projects;

/// <summary>
///     Command for creating a Project.
/// </summary>
/// <param name="Name">Name of the project</param>
/// <param name="RequestingUser">The user requesting this project be created</param>
/// <param name="OrganizationId">Optionally, the organization this project should belong to.</param>
public record CreateProjectCommand(string Name, BonesUser RequestingUser, Guid? OrganizationId = null) : IRequest<CommandResponse>;

internal sealed class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
{

}

internal sealed class CreateProjectHandler(ISender sender) : IRequestHandler<CreateProjectCommand, CommandResponse>
{
    public async Task<CommandResponse> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        if (!request.OrganizationId.HasValue)
        {
            return await sender.Send(new CreateProjectDbCommand(request.Name, request.RequestingUser), cancellationToken);
        }

        BonesOrganization? organization = await sender.Send(new GetOrganizationByIdDbQuery(request.OrganizationId.Value), cancellationToken);
        // Don't want to give away that this org doesn't exist, instead just return forbidden.
        if (organization is null)
        {
            return CommandResponse.Forbid();
        }

        const string perm = BonesClaimTypes.Role.Project.CREATE_PROJECT;
        bool? hasOrganizationPermission =
            await sender.Send(new UserHasOrganizationPermissionQuery(organization.Id, request.RequestingUser, perm), cancellationToken);

        if (hasOrganizationPermission != true)
        {
            return CommandResponse.Forbid();
        }

        return await sender.Send(new CreateProjectDbCommand(request.Name, request.RequestingUser, organization), cancellationToken);
    }
}