namespace Bones.Database.Operations.ProjectManagement.Projects.GetProjectsByOwnerDb;

internal sealed class GetProjectsByOwnerDbQueryValidator : AbstractValidator<GetProjectsByOwnerDbQuery>
{
    public override Task<ValidationResult> ValidateAsync(ValidationContext<GetProjectsByOwnerDbQuery> context, CancellationToken cancellation = new())
    {
        RuleFor(x => x.OwnerType).NotNull().IsInEnum();
        RuleFor(x => x.OwnerId).NotNull().NotEqual(Guid.Empty);

        return base.ValidateAsync(context, cancellation);
    }
}