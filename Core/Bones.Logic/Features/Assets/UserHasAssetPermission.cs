using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.AssetManagement;
using Bones.Database.Operations.AssetManagement;
using Bones.Logic.Features.Projects;

namespace Bones.Logic.Features.Assets;

/// <inheritdoc />
public class UserHasAssetPermission(ISender sender)
    : IRequestHandler<UserHasAssetPermission.Query, QueryResponse<bool>>
{
    /// <summary>
    ///   Checks if the user has permission to do the specified action in the initiative.
    /// </summary>
    /// <param name="AssetId"></param>
    /// <param name="User"></param>
    /// <param name="Claim"></param>
    public sealed record Query(Guid AssetId, BonesUser User, string Claim) : IRequest<QueryResponse<bool>>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Query>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.AssetId).NotNull().NotEqual(Guid.Empty);
            RuleFor(x => x.User).NotNull();
            RuleFor(x => x.Claim).NotNull().NotEmpty().Custom((claim, ctx) =>
            {
                if (claim.Contains('|'))
                {
                    ctx.AddFailure("Claim contains '|', this means you probably called Get*ClaimType(). Don't do that, just pass in the claim name.");
                }
            });
        }
    }

    /// <inheritdoc />
    public async Task<QueryResponse<bool>> Handle(Query request, CancellationToken cancellationToken)
    {
        Asset? asset = await sender.Send(new GetAssetByIdDb.Query(request.AssetId), cancellationToken);
        if (asset is null)
        {
            return QueryResponse<bool>.Fail("Asset not found");
        }

        bool? projectPermission = await sender.Send(
            new UserHasProjectPermission.Query(asset.Project.Id, request.User, request.Claim),
            cancellationToken);

        if (projectPermission == true)
        {
            return true;
        }

        return false;
    }
}
