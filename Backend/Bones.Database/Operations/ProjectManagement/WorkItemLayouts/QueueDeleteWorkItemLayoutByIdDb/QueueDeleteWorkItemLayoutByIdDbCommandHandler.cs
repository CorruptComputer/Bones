namespace Bones.Database.Operations.ProjectManagement.WorkItemLayouts.QueueDeleteWorkItemLayoutByIdDb;

internal sealed class QueueDeleteWorkItemLayoutByIdDbCommandHandler : AbstractValidator<QueueDeleteWorkItemLayoutByIdDbCommand>
{
    public override Task<ValidationResult> ValidateAsync(ValidationContext<QueueDeleteWorkItemLayoutByIdDbCommand> context, CancellationToken cancellation = new CancellationToken())
    {
        RuleFor(x => x.LayoutId).NotNull().NotEqual(Guid.Empty);
        
        return base.ValidateAsync(context, cancellation);
    }
}