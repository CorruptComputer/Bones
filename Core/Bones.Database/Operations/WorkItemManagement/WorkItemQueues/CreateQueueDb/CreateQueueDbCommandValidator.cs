namespace Bones.Database.Operations.WorkItemManagement.WorkItemQueues.CreateQueueDb;

internal class CreateQueueDbCommandValidator : AbstractValidator<CreateQueueDbCommand>
{
    public override Task<ValidationResult> ValidateAsync(ValidationContext<CreateQueueDbCommand> context, CancellationToken cancellation = new())
    {
        RuleFor(x => x.Name).NotNull().NotEmpty();
        RuleFor(x => x.InitiativeId).NotNull().NotEqual(Guid.Empty);

        return base.ValidateAsync(context, cancellation);
    }
}