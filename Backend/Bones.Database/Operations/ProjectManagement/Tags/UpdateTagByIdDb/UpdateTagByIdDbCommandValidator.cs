namespace Bones.Database.Operations.ProjectManagement.Tags.UpdateTagByIdDb;

internal sealed class UpdateTagByIdDbCommandValidator : AbstractValidator<UpdateTagByIdDbCommand>
{
    public override Task<ValidationResult> ValidateAsync(ValidationContext<UpdateTagByIdDbCommand> context, CancellationToken cancellation = new())
    {
        RuleFor(x => x.TagId).NotNull().NotEqual(Guid.Empty);
        RuleFor(x => x.Name).NotNull().NotEmpty();
        
        return base.ValidateAsync(context, cancellation);
    }
}