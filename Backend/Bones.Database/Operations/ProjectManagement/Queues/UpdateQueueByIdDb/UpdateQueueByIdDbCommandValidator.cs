namespace Bones.Database.Operations.ProjectManagement.Queues.UpdateQueueByIdDb;

internal class UpdateQueueByIdDbCommandValidator : AbstractValidator<UpdateQueueByIdDbCommand>
{
    public override Task<ValidationResult> ValidateAsync(ValidationContext<UpdateQueueByIdDbCommand> context, CancellationToken cancellation = new())
    {
        RuleFor(x => x.QueueId).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.NewName).NotNull().NotEmpty();
        
        return base.ValidateAsync(context, cancellation);
    }
}