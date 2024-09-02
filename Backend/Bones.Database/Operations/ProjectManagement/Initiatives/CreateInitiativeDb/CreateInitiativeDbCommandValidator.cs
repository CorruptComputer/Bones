namespace Bones.Database.Operations.ProjectManagement.Initiatives.CreateInitiativeDb;

internal sealed class CreateInitiativeDbCommandValidator : AbstractValidator<CreateInitiativeDbCommand>
{
    public override Task<ValidationResult> ValidateAsync(ValidationContext<CreateInitiativeDbCommand> context, CancellationToken cancellation = new())
    {
        RuleFor(x => x.ProjectId).NotNull().NotEqual(Guid.Empty);
        
        return base.ValidateAsync(context, cancellation);
    }
}