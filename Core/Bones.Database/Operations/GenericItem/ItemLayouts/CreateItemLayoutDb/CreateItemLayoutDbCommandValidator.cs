namespace Bones.Database.Operations.GenericItem.ItemLayouts.CreateItemLayoutDb;

internal class CreateItemLayoutDbCommandValidator : AbstractValidator<CreateItemLayoutDbCommand>
{
    public override Task<ValidationResult> ValidateAsync(ValidationContext<CreateItemLayoutDbCommand> context, CancellationToken cancellation = new())
    {
        RuleFor(x => x.Name).NotNull().NotEmpty();
        RuleFor(x => x.ProjectId).NotNull().NotEqual(Guid.Empty);

        return base.ValidateAsync(context, cancellation);
    }
}