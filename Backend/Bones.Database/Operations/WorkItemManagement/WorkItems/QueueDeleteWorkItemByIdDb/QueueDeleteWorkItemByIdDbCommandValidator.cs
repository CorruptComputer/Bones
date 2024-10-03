namespace Bones.Database.Operations.WorkItemManagement.WorkItems.QueueDeleteWorkItemByIdDb;

internal class QueueDeleteWorkItemByIdDbCommandValidator : AbstractValidator<QueueDeleteWorkItemByIdDbCommand>
{
    public override Task<ValidationResult> ValidateAsync(ValidationContext<QueueDeleteWorkItemByIdDbCommand> context, CancellationToken cancellation = new())
    {
        RuleFor(x => x.WorkItemId).NotNull().NotEqual(Guid.Empty);

        return base.ValidateAsync(context, cancellation);
    }
}