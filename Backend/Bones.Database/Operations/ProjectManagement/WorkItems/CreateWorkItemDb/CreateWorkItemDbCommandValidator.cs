namespace Bones.Database.Operations.ProjectManagement.WorkItems.CreateWorkItemDb;

internal sealed class CreateWorkItemDbCommandValidator : AbstractValidator<CreateWorkItemDbCommand>
{
    public override Task<ValidationResult> ValidateAsync(ValidationContext<CreateWorkItemDbCommand> context, CancellationToken cancellation = new())
    {
        RuleFor(x => x.Name).NotNull().NotEmpty();
        RuleFor(x => x.QueueId).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.WorkItemLayoutVersionId).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.Values).NotNull().ChildRules(dict =>
        {
            dict.RuleForEach(x => x.Keys).NotNull().NotEmpty();
        });

        return base.ValidateAsync(context, cancellation);
    }
}