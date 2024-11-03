using Bones.Shared.Backend.Enums;

namespace Bones.Backend.Features.ProjectManagement.Projects.GetProjectsByOwner;

internal sealed class GetProjectsByOwnerHandler(ISender sender) : IRequestHandler<GetProjectsByOwnerQuery, QueryResponse<List<(Guid Id, string Name)>>>
{
    public async Task<QueryResponse<List<(Guid Id, string Name)>>> Handle(GetProjectsByOwnerQuery request, CancellationToken cancellationToken)
    {
        if (request.OwnerType == OwnershipType.User
            && request.OwnerId == request.RequestingUser.Id)
        {
            throw new NotImplementedException();
        }
        else if (request.OwnerType == OwnershipType.Organization)
        {
            // TODO
            await sender.Send(new(), cancellationToken);
        }

        return QueryResponse<List<(Guid Id, string Name)>>.Forbid();
    }
}