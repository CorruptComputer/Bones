using Bones.Database.DbSets.AccountManagement;
using Bones.Database.Operations.ProjectManagement.Projects.GetProjectsByOwnerDb;
using Bones.Shared.Backend.Enums;

namespace Bones.Logic.Features.Projects.Projects;

/// <summary>
/// 
/// </summary>
/// <param name="OwnerType"></param>
/// <param name="OwnerId"></param>
/// <param name="RequestingUser"></param>
public sealed record GetProjectsByOwnerQuery(OwnershipType OwnerType, Guid OwnerId, BonesUser RequestingUser) : IRequest<QueryResponse<Dictionary<Guid, string>>>;

internal sealed class GetProjectsByOwnerQueryValidator : AbstractValidator<GetProjectsByOwnerQuery>
{
    public GetProjectsByOwnerQueryValidator()
    {
        RuleFor(x => x.OwnerType).NotNull().IsInEnum();
        RuleFor(x => x.OwnerId).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.RequestingUser).NotNull();
    }
}

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