using Bones.Backend.Features.AccountManagement.GetUserByClaimsPrincipal;
using Bones.Backend.Features.OrganizationManagement.UserHasOrganizationPermission;
using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.OrganizationManagement;
using Bones.Database.Operations.OrganizationManagement.GetOrganizationByIdDb;
using Bones.Database.Operations.ProjectManagement.Projects.CreateProjectDb;
using Bones.Shared.Consts;

namespace Bones.Backend.Features.ProjectManagement.Projects.CreateProject;

internal sealed class CreateProjectHandler(ISender sender) : IRequestHandler<CreateProjectCommand, CommandResponse>
{
    public async Task<CommandResponse> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        BonesUser? user = await sender.Send(new GetUserByClaimsPrincipalRequest(request.RequestingUser), cancellationToken);
        if (user == null)
        {
            return CommandResponse.Fail("User not found");
        }

        if (!request.OrganizationId.HasValue)
        {
            return await sender.Send(new CreateProjectDbCommand(request.Name, user), cancellationToken);
        }
        
        BonesOrganization? organization = await sender.Send(new GetOrganizationByIdDbQuery(request.OrganizationId.Value), cancellationToken);
        // Don't want to give away that this org doesn't exist, instead just return forbidden.
        if (organization is null)
        {
            return CommandResponse.Forbid();
        }

        const string perm = BonesClaimTypes.Role.Organization.Project.CREATE_PROJECT;
        bool? hasOrganizationPermission =
            await sender.Send(new UserHasOrganizationPermissionQuery(organization.Id, user, perm), cancellationToken);

        if (hasOrganizationPermission != true)
        {
            return CommandResponse.Forbid();
        }
        
        return await sender.Send(new CreateProjectDbCommand(request.Name, user, organization), cancellationToken);
    }
}