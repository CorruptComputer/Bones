namespace Bones.Database.Operations.WorkItemManagement.WorkItems.CreateWorkItemDb;

internal sealed class CreateWorkItemDbCommandValidator : AbstractValidator<CreateWorkItemDbCommand>
{
    public override Task<ValidationResult> ValidateAsync(ValidationContext<CreateWorkItemDbCommand> context, CancellationToken cancellation = new())
    {
        RuleFor(x => x.Name).NotNull().NotEmpty();
        RuleFor(x => x.QueueId).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.ItemLayoutId).NotNull().NotEqual(Guid.Empty);

        return base.ValidateAsync(context, cancellation);
    }
}