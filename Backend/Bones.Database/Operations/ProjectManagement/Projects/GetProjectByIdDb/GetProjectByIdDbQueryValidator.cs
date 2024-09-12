namespace Bones.Database.Operations.ProjectManagement.Projects.GetProjectByIdDb;

internal sealed class GetProjectByIdDbQueryValidator : AbstractValidator<GetProjectByIdDbQuery>
{
    public override Task<ValidationResult> ValidateAsync(ValidationContext<GetProjectByIdDbQuery> context, CancellationToken cancellation = new())
    {
        RuleFor(x => x.ProjectId).NotNull().NotEqual(Guid.Empty);

        return base.ValidateAsync(context, cancellation);
    }
}