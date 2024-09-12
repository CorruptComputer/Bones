namespace Bones.Database.Operations.ProjectManagement.WorkItems.CreateWorkItemVersionDb;

internal sealed class CreateWorkItemVersionDbCommandValidator : AbstractValidator<CreateWorkItemVersionDbCommand>
{
    public override Task<ValidationResult> ValidateAsync(ValidationContext<CreateWorkItemVersionDbCommand> context, CancellationToken cancellation = new())
    {
        RuleFor(x => x.WorkItemId).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.WorkItemLayoutVersionId).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.Values).NotNull().ChildRules(dict =>
        {
            dict.RuleForEach(x => x.Keys).NotNull().NotEmpty();
        });

        return base.ValidateAsync(context, cancellation);
    }
}