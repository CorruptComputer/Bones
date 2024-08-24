using FluentValidation.Results;

namespace Bones.Api.Features.AccountManagement.GetUserByClaimsPrincipal;

public sealed class GetUserByClaimsPrincipalRequestValidator : AbstractValidator<GetUserByClaimsPrincipalRequest>
{
    public override Task<ValidationResult> ValidateAsync(ValidationContext<GetUserByClaimsPrincipalRequest> context, CancellationToken cancellation = new())
    {
        RuleFor(x => x.ClaimsPrincipal).NotNull().WithMessage("Claims Principal cannot be null");
        
        return base.ValidateAsync(context, cancellation);
    }
}