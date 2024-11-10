using FluentValidation.Results;

namespace Bones.Backend.Features.Projects.Projects.GetProjectsByOwner;

internal sealed class GetProjectsByOwnerQueryValidator : AbstractValidator<GetProjectsByOwnerQuery>
{
    public override Task<ValidationResult> ValidateAsync(ValidationContext<GetProjectsByOwnerQuery> context, CancellationToken cancellation = new())
    {
        RuleFor(x => x.OwnerType).NotNull().IsInEnum();
        RuleFor(x => x.OwnerId).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.RequestingUser).NotNull();

        return base.ValidateAsync(context, cancellation);
    }
}