using Bones.Database.Operations.ProjectManagement.Projects.GetProjectsByOwnerDb;
using Bones.Shared.Backend.Enums;

namespace Bones.Backend.Features.Projects.Projects.GetProjectsByOwner;

internal sealed class GetProjectsByOwnerHandler(ISender sender) : IRequestHandler<GetProjectsByOwnerQuery, QueryResponse<Dictionary<Guid, string>>>
{
    public async Task<QueryResponse<Dictionary<Guid, string>>> Handle(GetProjectsByOwnerQuery request, CancellationToken cancellationToken)
    {
        if (request.OwnerType == OwnershipType.User
            && request.OwnerId == request.RequestingUser.Id)
        {
            List<Database.DbSets.ProjectManagement.Project>? projects = await sender.Send(new GetProjectsByOwnerDbQuery(OwnershipType.User, request.RequestingUser.Id), cancellationToken);

            if (projects is null)
            {
                return QueryResponse<Dictionary<Guid, string>>.Fail("DB failed :(");
            }

            return projects.ToDictionary(project => project.Id, project => project.Name);
        }
        else if (request.OwnerType == OwnershipType.Organization)
        {
            // TODO: Not implemented, will fail. Need to also check perms here before requesting DB
            await sender.Send(new GetProjectsByOwnerDbQuery(OwnershipType.Organization, request.OwnerId), cancellationToken);
        }

        return QueryResponse<Dictionary<Guid, string>>.Forbid();
    }
}