using FluentValidation.Results;

namespace Bones.Logic.Features.Projects.Projects.GetProjectsUserCanAccess;

internal sealed class GetProjectsUserCanAccessQueryValidator : AbstractValidator<GetProjectsUserCanAccessQuery>
{
    public override Task<ValidationResult> ValidateAsync(ValidationContext<GetProjectsUserCanAccessQuery> context, CancellationToken cancellation = new())
    {
        RuleFor(x => x.RequestingUser).NotNull();

        return base.ValidateAsync(context, cancellation);
    }
}