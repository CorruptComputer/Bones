namespace Bones.Database.Operations.ProjectManagement.WorkItems.UpdateWorkItemByIdDb;

internal sealed class UpdateWorkItemByIdDbCommandValidator : AbstractValidator<UpdateWorkItemByIdDbCommand>
{
    public override Task<ValidationResult> ValidateAsync(ValidationContext<UpdateWorkItemByIdDbCommand> context, CancellationToken cancellation = new())
    {
        RuleFor(x => x.WorkItemId).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.Name).NotNull().NotEmpty();
        RuleFor(x => x.QueueId).NotNull().NotEqual(Guid.Empty);
        RuleForEach(x => x.TagIds).NotNull().NotEqual(Guid.Empty);
        
        return base.ValidateAsync(context, cancellation);
    }
}