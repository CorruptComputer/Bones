using FluentValidation.Results;

namespace Bones.Backend.Features.AccountManagement.GetUserByClaimsPrincipal;

internal sealed class GetUserByClaimsPrincipalQueryValidator : AbstractValidator<GetUserByClaimsPrincipalQuery>
{
    public override Task<ValidationResult> ValidateAsync(ValidationContext<GetUserByClaimsPrincipalQuery> context, CancellationToken cancellation = new())
    {
        RuleFor(x => x.ClaimsPrincipal).NotNull().WithMessage("Claims Principal cannot be null");

        return base.ValidateAsync(context, cancellation);
    }
}