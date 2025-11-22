using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.Items;
using Bones.Database.Operations.Items;
using Bones.Logic.Features.Projects;
using Bones.Shared.Consts;

namespace Bones.Logic.Features.Item;

/// <inheritdoc />
public sealed class GetItemFieldsByProject(ISender sender) : IRequestHandler<GetItemFieldsByProject.Query, QueryResponse<List<ItemField>>>
{
    /// <summary>
    ///   Query for getting the item fields in a project
    /// </summary>
    /// <param name="ProjectId">Internal ID of the project</param>
    /// <param name="RequestingUser">The user requesting this</param>
    public record Query(Guid ProjectId, BonesUser RequestingUser) : IRequest<QueryResponse<List<ItemField>>>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Query>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.ProjectId).NotNull().NotEqual(Guid.Empty);
            RuleFor(x => x.RequestingUser).NotNull();
        }
    }

    /// <inheritdoc />
    public async Task<QueryResponse<List<ItemField>>> Handle(Query request, CancellationToken cancellationToken)
    {
        const string perm = BonesClaimTypes.Role.Project.VIEW_PROJECT;
        bool? hasProjectPermission =
            await sender.Send(new UserHasProjectPermission.Query(request.ProjectId, request.RequestingUser, perm), cancellationToken);

        if (hasProjectPermission != true)
        {
            return QueryResponse<List<ItemField>>.Forbid();
        }

        return await sender.Send(new GetItemFieldsByProjectDb.Query(request.ProjectId), cancellationToken);
    }
}
