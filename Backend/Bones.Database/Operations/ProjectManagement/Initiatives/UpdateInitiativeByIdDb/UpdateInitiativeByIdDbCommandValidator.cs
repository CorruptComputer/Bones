namespace Bones.Database.Operations.ProjectManagement.Initiatives.UpdateInitiativeByIdDb;

internal class UpdateInitiativeByIdDbCommandValidator : AbstractValidator<UpdateInitiativeByIdDbCommand>
{
    public override Task<ValidationResult> ValidateAsync(ValidationContext<UpdateInitiativeByIdDbCommand> context, CancellationToken cancellation = new())
    {
        RuleFor(x => x.InitiativeId).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.NewName).NotNull().NotEmpty();
        
        return base.ValidateAsync(context, cancellation);
    }
}