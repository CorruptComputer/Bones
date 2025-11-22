using Bones.Database.DbSets.AccountManagement;
using Microsoft.AspNetCore.Identity;

namespace Bones.Logic.Features.Organizations;

/// <inheritdoc />
public sealed class UserIdHasOrganizationPermission : IRequestHandler<UserIdHasOrganizationPermission.Query, QueryResponse<bool>>
{
    /// <summary>
    ///   Checks if the user has permission to do the specified action in the organization.
    /// </summary>
    /// <param name="OrganizationId"></param>
    /// <param name="UserId"></param>
    /// <param name="Claim"></param>
    public sealed record Query(Guid OrganizationId, Guid UserId, string Claim) : IRequest<QueryResponse<bool>>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Query>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.OrganizationId).NotNull().NotEqual(Guid.Empty);
            RuleFor(x => x.UserId).NotNull().NotEqual(Guid.Empty);
            RuleFor(x => x.Claim).NotEmpty().Custom((claim, ctx) =>
            {
                if (claim.Contains('|'))
                {
                    ctx.AddFailure("Claim contains '|', this means you probably called GetOrganizationWideClaimType(). Don't do that, just pass in the claim name.");
                }
            });
        }
    }

    /// <inheritdoc />
    public Task<QueryResponse<bool>> Handle(Query request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
