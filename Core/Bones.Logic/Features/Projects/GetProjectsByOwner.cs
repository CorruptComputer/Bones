using Bones.Database.DbSets.AccountManagement;
using Bones.Database.Operations.ProjectManagement.Projects;
using Bones.Shared.Backend.Enums;

namespace Bones.Logic.Features.Projects;

/// <inheritdoc />
public sealed class GetProjectsByOwner(ISender sender) : IRequestHandler<GetProjectsByOwner.Query, QueryResponse<Dictionary<Guid, string>>>
{
    /// <summary>
    ///   Query to get all projects for a given owner (user or organization)
    /// </summary>
    /// <param name="OwnerType"></param>
    /// <param name="OwnerId"></param>
    /// <param name="RequestingUser"></param>
    public sealed record Query(OwnershipType OwnerType, Guid OwnerId, BonesUser RequestingUser) : IRequest<QueryResponse<Dictionary<Guid, string>>>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Query>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.OwnerType).NotNull().IsInEnum();
            RuleFor(x => x.OwnerId).NotNull().NotEqual(Guid.Empty);
            RuleFor(x => x.RequestingUser).NotNull();
        }
    }

    /// <inheritdoc />
    public async Task<QueryResponse<Dictionary<Guid, string>>> Handle(Query request, CancellationToken cancellationToken)
    {
        if (request.OwnerType == OwnershipType.User
            && request.OwnerId == request.RequestingUser.Id)
        {
            List<Database.DbSets.ProjectManagement.Project>? projects = await sender.Send(new GetProjectsByOwnerDb.Query(OwnershipType.User, request.RequestingUser.Id), cancellationToken);

            if (projects is null)
            {
                return QueryResponse<Dictionary<Guid, string>>.Fail("DB failed :(");
            }

            return projects.ToDictionary(project => project.Id, project => project.Name);
        }
        else if (request.OwnerType == OwnershipType.Organization)
        {
            // TODO: Not implemented, will fail. Need to also check perms here before requesting DB
            await sender.Send(new GetProjectsByOwnerDb.Query(OwnershipType.Organization, request.OwnerId), cancellationToken);
        }

        return QueryResponse<Dictionary<Guid, string>>.Forbid();
    }
}