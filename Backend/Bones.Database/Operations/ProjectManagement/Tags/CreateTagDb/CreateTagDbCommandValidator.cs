namespace Bones.Database.Operations.ProjectManagement.Tags.CreateTagDb;

internal sealed class CreateTagDbCommandValidator : AbstractValidator<CreateTagDbCommand>
{
    public override Task<ValidationResult> ValidateAsync(ValidationContext<CreateTagDbCommand> context, CancellationToken cancellation = new())
    {
        RuleFor(x => x.Name).NotNull().NotEmpty();
        
        return base.ValidateAsync(context, cancellation);
    }
}