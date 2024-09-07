namespace Bones.Database.Operations.ProjectManagement.WorkItemLayouts.UpdateWorkItemLayoutByIdDb;

internal sealed class UpdateWorkItemLayoutByIdDbCommandValidator : AbstractValidator<UpdateWorkItemLayoutByIdDbCommand>
{
    public override Task<ValidationResult> ValidateAsync(ValidationContext<UpdateWorkItemLayoutByIdDbCommand> context, CancellationToken cancellation = new CancellationToken())
    {
        RuleFor(x => x.LayoutId).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.NewName).NotNull().NotEmpty();
        
        return base.ValidateAsync(context, cancellation);
    }
}