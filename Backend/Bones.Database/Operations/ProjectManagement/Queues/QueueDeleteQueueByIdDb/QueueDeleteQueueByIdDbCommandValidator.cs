namespace Bones.Database.Operations.ProjectManagement.Queues.QueueDeleteQueueByIdDb;

internal sealed class QueueDeleteQueueByIdDbCommandValidator : AbstractValidator<QueueDeleteQueueByIdDbCommand>
{
    public override Task<ValidationResult> ValidateAsync(ValidationContext<QueueDeleteQueueByIdDbCommand> context, CancellationToken cancellation = new())
    {
        RuleFor(x => x.QueueId).NotNull().NotEqual(Guid.Empty);
        
        return base.ValidateAsync(context, cancellation);
    }
}