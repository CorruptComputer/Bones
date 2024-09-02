namespace Bones.Database.Operations.AssetManagement.AssetLayouts.CreateAssetLayoutDb;

internal class CreateAssetLayoutDbCommandValidator : AbstractValidator<CreateAssetLayoutDbCommand>
{
    public override Task<ValidationResult> ValidateAsync(ValidationContext<CreateAssetLayoutDbCommand> context, CancellationToken cancellation = new())
    {
        RuleFor(x => x.OwnershipType).NotNull().IsInEnum();
        RuleFor(x => x.OwnerId).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.Name).NotNull().NotEmpty();
        
        return base.ValidateAsync(context, cancellation);
    }
}