namespace Bones.Database.Operations.ProjectManagement.WorkItemLayouts.CreateWorkItemLayoutDb;

internal sealed class CreateWorkItemLayoutDbCommandValidator : AbstractValidator<CreateWorkItemLayoutDbCommand>
{
    public override Task<ValidationResult> ValidateAsync(ValidationContext<CreateWorkItemLayoutDbCommand> context, CancellationToken cancellation = new())
    {
        RuleFor(x => x.ProjectId).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.Name).NotNull().NotEmpty();
        
        return base.ValidateAsync(context, cancellation);
    }
}