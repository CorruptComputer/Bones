using FluentValidation.Results;

namespace Bones.Backend.Features.Projects.Projects.GetProjectById;

internal sealed class GetProjectByIdQueryValidator : AbstractValidator<GetProjectByIdQuery>
{
    public override Task<ValidationResult> ValidateAsync(ValidationContext<GetProjectByIdQuery> context, CancellationToken cancellation = new())
    {
        RuleFor(x => x.RequestingUser).NotNull();

        return base.ValidateAsync(context, cancellation);
    }
}