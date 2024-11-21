namespace Bones.Database.Operations.ProjectManagement.Initiatives.QueueDeleteInitiativeByIdDb;

internal class QueueDeleteInitiativeByIdDbCommandValidator : AbstractValidator<QueueDeleteInitiativeByIdDbCommand>
{
    public override Task<ValidationResult> ValidateAsync(ValidationContext<QueueDeleteInitiativeByIdDbCommand> context, CancellationToken cancellation = new())
    {
        RuleFor(x => x.InitiativeId).NotNull().NotEqual(Guid.Empty);

        return base.ValidateAsync(context, cancellation);
    }
}