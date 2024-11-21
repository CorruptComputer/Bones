namespace Bones.Database.Operations.WorkItemManagement.WorkItemQueues.UpdateQueueByIdDb;

internal class UpdateQueueByIdDbCommandValidator : AbstractValidator<UpdateQueueByIdDbCommand>
{
    public override Task<ValidationResult> ValidateAsync(ValidationContext<UpdateQueueByIdDbCommand> context, CancellationToken cancellation = new())
    {
        RuleFor(x => x.QueueId).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.NewName).NotNull().NotEmpty();

        return base.ValidateAsync(context, cancellation);
    }
}