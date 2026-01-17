using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.Items;
using Bones.Database.Operations.Items;
using Bones.Logic.Features.Projects;
using Bones.Shared.Consts;

namespace Bones.Logic.Features.Items;

/// <inheritdoc />
public sealed class GetItemVersionById(ISender sender) : IRequestHandler<GetItemVersionById.Query, QueryResponse<ItemVersion?>>
{
    /// <summary>
    ///   Query for getting an item version by its internal ID
    /// </summary>
    /// <param name="ItemId">Internal ID of the item version</param>
    /// <param name="RequestingUser">The user requesting this</param>
    /// <param name="IncludeLayoutVersion">Whether to include the related layout version</param>
    /// <param name="IncludeValues">Whether to include the item values</param>
    /// <param name="IncludeAssignees">Whether to include the assignees</param>
    public record Query(Guid ItemId, BonesUser RequestingUser, bool IncludeLayoutVersion = false, bool IncludeValues = false, bool IncludeAssignees = false) : IRequest<QueryResponse<ItemVersion?>>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Query>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.ItemId).NotNull().NotEqual(Guid.Empty);
            RuleFor(x => x.RequestingUser).NotNull();
        }
    }

    /// <inheritdoc />
    public async Task<QueryResponse<ItemVersion?>> Handle(Query request, CancellationToken cancellationToken)
    {
        ItemVersion? itemVersion = await sender.Send(new GetItemVersionByIdDb.Query(request.ItemId, request.IncludeLayoutVersion, request.IncludeValues, request.IncludeAssignees), cancellationToken);
        if (itemVersion == null)
        {
            return QueryResponse<ItemVersion?>.Fail("Item version not found");
        }

        const string perm = BonesClaimTypes.Role.Project.VIEW_PROJECT;
        bool? hasProjectPermission =
            await sender.Send(new UserHasProjectPermission.Query(itemVersion.Item!.ProjectId, request.RequestingUser, perm), cancellationToken);

        if (hasProjectPermission != true)
        {
            return QueryResponse<ItemVersion?>.Forbid();
        }

        return itemVersion;
    }
}