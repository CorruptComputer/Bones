namespace Bones.Database.Operations.AssetManagement.AssetLayouts.UpdateAssetLayoutByIdDb;

internal class UpdateAssetLayoutByIdDbCommandValidator : AbstractValidator<UpdateAssetLayoutByIdDbCommand>
{
    public override Task<ValidationResult> ValidateAsync(ValidationContext<UpdateAssetLayoutByIdDbCommand> context, CancellationToken cancellation = new())
    {
        RuleFor(x => x.AssetLayoutId).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.NewName).NotNull().NotEmpty();
        
        return base.ValidateAsync(context, cancellation);
    }
}