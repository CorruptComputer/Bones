using FluentValidation.Results;

namespace Bones.Backend.Features.OrganizationManagement.UserHasOrganizationPermission;

internal sealed class UserHasOrganizationPermissionQueryValidator : AbstractValidator<UserHasOrganizationPermissionQuery>
{
    public override Task<ValidationResult> ValidateAsync(ValidationContext<UserHasOrganizationPermissionQuery> context, CancellationToken cancellation = new())
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

        return base.ValidateAsync(context, cancellation);
    }
}