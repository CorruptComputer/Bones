namespace Bones.Database.Operations.ProjectManagement.WorkItems.QueueDeleteWorkItemVersionByIdDb;

internal class QueueDeleteWorkItemVersionByIdDbCommandValidator : AbstractValidator<QueueDeleteWorkItemVersionByIdDbCommand>
{
    public override Task<ValidationResult> ValidateAsync(ValidationContext<QueueDeleteWorkItemVersionByIdDbCommand> context, CancellationToken cancellation = new())
    {
        RuleFor(x => x.WorkItemVersionId).NotNull().NotEqual(Guid.Empty);
        
        return base.ValidateAsync(context, cancellation);
    }
}