using Bones.Database.DbSets.AccountManagement;

namespace Bones.Logic.Features.Organizations;

/// <summary>
///   Checks if the user has permission to do the specified action in the organization.
/// </summary>
/// <param name="OrganizationId"></param>
/// <param name="User"></param>
/// <param name="Claim"></param>
public sealed record UserHasOrganizationPermissionQuery(Guid OrganizationId, BonesUser User, string Claim) : IRequest<QueryResponse<bool>>;

internal sealed class UserHasOrganizationPermissionQueryValidator : AbstractValidator<UserHasOrganizationPermissionQuery>
{
    public UserHasOrganizationPermissionQueryValidator()
    {
        RuleFor(x => x.OrganizationId).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.User).NotNull();
        RuleFor(x => x.Claim).NotNull().NotEmpty().Custom((claim, ctx) =>
        {
            if (claim.Contains('|'))
            {
                ctx.AddFailure("Claim contains '|', this means you probably called GetOrganizationWideClaimType(). Don't do that, just pass in the claim name.");
            }
        });
    }
}