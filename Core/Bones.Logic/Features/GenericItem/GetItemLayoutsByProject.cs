using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.GenericItems;
using Bones.Database.Operations.GenericItem;
using Bones.Logic.Features.Projects;
using Bones.Shared.Consts;

namespace Bones.Logic.Features.GenericItem;

/// <inheritdoc />
public sealed class GetItemLayoutsByProject(ISender sender) : IRequestHandler<GetItemLayoutsByProject.Query, QueryResponse<List<GenericItemLayout>>>
{
    /// <summary>
    ///   Query for getting the item layouts in a project
    /// </summary>
    /// <param name="ProjectId">Internal ID of the project</param>
    /// <param name="RequestingUser">The user requesting this</param>
    public record Query(Guid ProjectId, BonesUser RequestingUser) : IRequest<QueryResponse<List<GenericItemLayout>>>;

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
    public async Task<QueryResponse<List<GenericItemLayout>>> Handle(Query request, CancellationToken cancellationToken)
    {
        const string perm = BonesClaimTypes.Role.Project.VIEW_PROJECT;
        bool? hasProjectPermission =
            await sender.Send(new UserHasProjectPermission.Query(request.ProjectId, request.RequestingUser, perm), cancellationToken);

        if (hasProjectPermission != true)
        {
            return QueryResponse<List<GenericItemLayout>>.Forbid();
        }

        return await sender.Send(new GetItemLayoutsByProjectDb.Query(request.ProjectId), cancellationToken);
    }
}
