namespace Bones.Database.Operations.GenericItem.ItemLayouts.UpdateItemLayoutByIdDb;

internal class UpdateItemLayoutByIdDbCommandValidator : AbstractValidator<UpdateItemLayoutByIdDbCommand>
{
    public override Task<ValidationResult> ValidateAsync(ValidationContext<UpdateItemLayoutByIdDbCommand> context, CancellationToken cancellation = new())
    {
        RuleFor(x => x.ItemLayoutId).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.NewName).NotNull().NotEmpty();

        return base.ValidateAsync(context, cancellation);
    }
}