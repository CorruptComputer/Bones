namespace Bones.Database.Operations.ProjectManagement.Projects.GetProjectByIdDb;

internal sealed class GetProjectByIdDbCommandValidator : AbstractValidator<GetProjectByIdDbCommand>
{
    public override Task<ValidationResult> ValidateAsync(ValidationContext<GetProjectByIdDbCommand> context, CancellationToken cancellation = new CancellationToken())
    {
        RuleFor(x => x.ProjectId).NotNull().NotEqual(Guid.Empty);
        
        return base.ValidateAsync(context, cancellation);
    }
}