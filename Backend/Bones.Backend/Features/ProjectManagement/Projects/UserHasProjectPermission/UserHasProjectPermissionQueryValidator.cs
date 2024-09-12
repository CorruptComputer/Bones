using FluentValidation.Results;

namespace Bones.Backend.Features.ProjectManagement.Projects.UserHasProjectPermission;

internal sealed class UserHasProjectPermissionQueryValidator : AbstractValidator<UserHasProjectPermissionQuery>
{
    public override Task<ValidationResult> ValidateAsync(ValidationContext<UserHasProjectPermissionQuery> context, CancellationToken cancellation = new())
    {
        RuleFor(x => x.ProjectId).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.User).NotNull();
        RuleFor(x => x.Claim).NotNull().NotEmpty().Custom((claim, ctx) =>
        {
            if (claim.Contains('|'))
            {
                ctx.AddFailure("Claim contains '|', this means you probably called GetProjectClaimType(). Don't do that, just pass in the claim name.");
            }
        });

        return base.ValidateAsync(context, cancellation);
    }
}